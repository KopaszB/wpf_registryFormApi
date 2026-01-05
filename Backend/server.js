const express = require('express'); // importálom az express keretrendszert, hogy tudjak API-kat kezelni
const mysql = require('mysql2');    // importálom a mysql könyvtárat, hogy tudjak sql adatbázisokat kezelni
const app = express();              // létrehozok egy app nevű webszerver alkalmazást

const port = 3000;
const host = 'localhost';


//egyszerű adatlekérés a szervertől
app.get('/',(req,res)=>{
    res.send('A szerver fut!')
});

app.use(express.json());    //A JSON formátumú body feldolgozása
app.use(express.urlencoded({ extended: true }));    //Űrlapból érkező (form) adat feldolgozását

//beállítjuk a kapcsolatot az adatbázissal
const connection = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'regisztracio',
    dateStrings: true //dátumadatról leválassza az órát és a percet
});

//adatbáziskapcsolat ellenőrzése
app.get('/test',(req,res)=>{
    connection.query('SELECT 1',(err,results)=>{
        if (err) {
            console.error('Adatbázis hiba!',err);
            return res.status(500).send('Nem sikerült kapcsolódni az adatbázishoz!');
        }
        res.send('A kapcsolat az adatbázissal létrejött!')
    })
});

//összes adat lekérése
app.get('/all',(req,res)=>{
    connection.query('SELECT * FROM felhasznalok',(err,result)=>{
        if (err) {
            console.error('Adatbázis hiba!',err);
            return res.status(500).json({error: 'Hiba a lekérdezés során!'});
        }
        if (result.length===0) {
            return res.status(404).json({error:'Nincs adat!'})
        }
        return res.status(200).json(result)
    })
});
// új dolgozó felvétele
app.post('/users', (req, res) => {
    console.log("BODY:", req.body);
    const { nev, email, szul_datum, jelszo } = req.body;


    if (!nev || !email || !szul_datum || !jelszo) {
        return res.status(400).json({ error: "Minden mezőt tölts ki!" });
    }

    connection.query('INSERT INTO felhasznalok (id, nev, email, szul_datum, jelszo) VALUES (NULL, ?, ?, ?, ?)', [nev, email, szul_datum, jelszo], (err, result) => {
        if (err) {
            return res.status(500).json({ error: 'Adatbázis hiba!' });
        }
        
        console.log(result);
        
        return res.status(201).json({
            message: 'Sikeres feltöltés!',
            id: result.insertId
        });
    });
});


//megadjuk, hogy a szerverünk hol fog futni
app.listen(port,host,()=>{
    console.log(`Listening at http://${host}:${port}`)
});

