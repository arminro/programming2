A feladathoz két féle képpen lehet bemenetet szolgáltatni
	txt file-ból a következõ szintaxist alkalmazva: nyereség (int);idõtartam (int:int);prioritás (0 - normál, 	1 - visszatérõ, 2 - sürgõs)
	a programon belülrõl újonnan érkezõ feladatként: nyereség (int);idõtartam (int:int);érkezés ideje 	(int:int)

szintaktikailag hibás inputok esetén a program hibát dob
a feladatok végrehajtásának szimulációja idõtartamarányos (1 óra 1 másodperc), azonban az újonnan érkezõ
feladatokat nem lehet valós idõben megadni, csak a szimuláció elõtt, az utasításokat követve

mivel a feladat külön nem tért ki erre, új sürgõs feladat érkezésekor, ha nem hajtható végre a feladat fél órán belül, mert éppen egy másik sürgõs feladat kerül feldolgozásra, akkor kivételt generál a szoftver
ellenkezõ esetben a feladatot félbeszakítja és áttér az új sürgõs feladat végrehajtására
 
a szimulációban szereplõ idõpontok kizárólag annak ellenõrzését könnyítik meg, hogy a program 8 órába osztja-e be
a feladatokat, az üresjáratot, vagy a megszakított feladatokat nem veszi figyelembe
mivel a feladat a beosztás sorrendjét tekinti, az újonnan érkezett feladatok a helyes sorrendben kerülnek végrehajtásra, azonban  a megjelenített kezdési idejük nem feltétlenül valós,  például ha kevés, rövid feladat került végrehajtásra a nap elején és egy új feladat érkezik a nap végén, akkor a szoftver úgy jeleníti meg, mintha közvetlenül a legutolsó feladat után kezdték volna meg annak végrehajtását akkor is, ha a megjelenített idõpont  korábbi mint a feladat megérkezése

