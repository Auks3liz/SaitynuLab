# SaitynuLab
# 1.	Projekto aprašymas
Kuriamas projektas – minimali patiekalų receptų sistema (Dish -> recipie -> comment). 
## 1.1.	Sistemos paskirtis
Sistema yra skirta vartotojams kurti bei atrasti juos dominančius patiekalų receptus:
  -	Vartotojas pagal tam tikras patiekalų kategorijas gali atrasti receptus, po kuriais galima palikti komentarus/atsiliepimus.
  -	Administratorius gali kurti ne tik patiekalų kategorijas, bet ir į jas pridėti atitinkamus receptus.
  -	Svečias gali tik stebėti esamus receptus.
## 1.2.	Funkciniai reikalavimai
Vienas iš funkcinių reikalavimų – galimybė valdyti visus tris resursus (dish, recipie, comment) atitinkamai pagal savo roles. Taip pat reikia realizuoti vartotojo autentifikaciją ir autorizaciją. 

# 2.	Technologijų aprašymas
Projekto ‚Frontend‘ bus kuriamas su React technologija. Duomenų bazė – PostgreSQL. Serverio pusė - naudojant .Net Core Web API

# 3. 	Naudotojo sąsaja
## 3.1 Pagrindinis/Meal puslaptis
### Wireframe
![image](https://github.com/user-attachments/assets/a05da562-1636-45f5-96ee-2bf0946b19a8)

### Realus pavyzdys
![image](https://github.com/user-attachments/assets/edd6899e-36e1-4c87-84fa-2a5c50c84891)

## 3.2 Atitinkamo recipie puslaptis
### Wireframe
![image](https://github.com/user-attachments/assets/8aacd9db-6168-4c5b-bd36-b7e324a67d34)

### Realus pavyzdys
![image](https://github.com/user-attachments/assets/5e86c6fd-2bca-49f7-afb5-7997520f996f)

## 3.3 Prisijungimo forma
### Wireframe
![image](https://github.com/user-attachments/assets/392b6bbf-d83b-46e1-8d45-b3ce29507fe0)

Panašaus principo wireframe pritaikomas ir registracijai, tačiau atsiranda papildomas email langas

### Realus pavyzdys (prisijungimas)
![image](https://github.com/user-attachments/assets/52ed2bdf-9d4d-42cb-8a41-cfbcaf486de1)

### Realus pavyzdys (registracija)
![image](https://github.com/user-attachments/assets/72486aae-f1f9-4ec9-aa53-720a7b985b82)

## 3.4 Prisijungimo forma
### Wireframe
![image](https://github.com/user-attachments/assets/495ce9eb-9e30-4ed9-8866-5a66cc54ff7d)

Panašaus principo wireframe pritaikomas ir kitiems kūrumo/redagavimo langams

### Realus pavyzdys (kuriamas naujas meal)
![image](https://github.com/user-attachments/assets/43ac33db-7258-4a86-aa36-857d497fa570)

### Realus pavyzdys (redaguojamas meal)
![image](https://github.com/user-attachments/assets/a8f1eff5-a492-4324-b796-2f5e084a4760)

### Realus pavyzdys (kuriamas naujas recipie)
![image](https://github.com/user-attachments/assets/6480dbab-3ca2-47d2-9848-08c3ee2740a7)

### Realus pavyzdys (redaguojamas recipie)
![image](https://github.com/user-attachments/assets/2a493bdf-ddc5-4801-b574-ff73d10f379a)

# 4.	API specifikacija



# 5. Išvados
1. Sukurtas forum API pasinaudojant REST principais su .NET
2. Duomenų bazei panaudotas PostgresSQL.
3. Klientinei daliai sukurti panaudotas React.js
4. Autorizacijai panaudoti JWT tokenai.
5. Kliento ir serverio dalys yra pasiekiamos per debesis.
6. Pateikta detali ataskaita.



