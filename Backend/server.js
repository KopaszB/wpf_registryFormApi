const express = require('express'); // importálom az express keretrendszert, hogy tudjak API-kat kezelni
const mysql = require('mysql2');    // importálom a mysql könyvtárat, hogy tudjak sql adatbázisokat kezelni
const app = express();              // létrehozok egy app nevű webszerver alkalmazást

const port = 3000;
const host = 'localhost';

//megadjuk, hogy a szerverünk hol fog futni
app.listen(port,host,()=>{
    console.log(`Listening at http://${host}:${port}`)
});

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
    database: 'regisztracio'
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
app.get('/users',(req,res)=>{
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
// új felhasználó felvétele
app.post('/users', (req, res) => {
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
// felhasználó módosítása (nem dinamikus)
app.put('/users/:id', (req, res) => {
    const id = Number(req.params.id);
    const { nev, email, szul_datum, jelszo } = req.body;

    if (!Number.isInteger(id) || id <= 0) {
        return res.status(400).json({ error: "Hibás ID!" });
    }

    // itt minden mezőt kötelezővé teszünk (tanuláshoz egyszerűbb)
    if (!nev || !email || !szul_datum || !jelszo) {
        return res.status(400).json({ error: "Minden mezőt tölts ki!" });
    }

    const sql = `
        UPDATE felhasznalok
        SET nev = ?, email = ?, szul_datum = ?, jelszo = ?
        WHERE id = ?
    `;

    connection.query(sql, [nev, email, szul_datum, jelszo, id], (err, result) => {
        if (err) {
            console.error("Adatbázis hiba!", err);
            return res.status(500).json({ error: "Adatbázis hiba!" });
        }

        if (result.affectedRows === 0) {
            return res.status(404).json({ error: "Nincs ilyen ID-jú felhasználó!" });
        }

        return res.status(200).json({ message: "Sikeres módosítás!" });
    });
});
//felhasználó törlése
app.delete('/users/:id', (req, res) => {
    const id = Number(req.params.id);

    if (!Number.isInteger(id) || id <= 0) {
        return res.status(400).json({ error: "Hibás ID!" });
    }

    connection.query('DELETE FROM felhasznalok WHERE id = ?', [id], (err, result) => {
        if (err) {
            console.error("Adatbázis hiba!", err);
            return res.status(500).json({ error: "Adatbázis hiba!" });
        }

        if (result.affectedRows === 0) {
            return res.status(404).json({ error: "Nincs ilyen ID-jú felhasználó!" });
        }

        return res.status(200).json({ message: "Sikeres törlés!" });
    });
});