A feladathoz k�t f�le k�ppen lehet bemenetet szolg�ltatni
	txt file-b�l a k�vetkez� szintaxist alkalmazva: nyeres�g (int);id�tartam (int:int);priorit�s (0 - norm�l, 	1 - visszat�r�, 2 - s�rg�s)
	a programon bel�lr�l �jonnan �rkez� feladatk�nt: nyeres�g (int);id�tartam (int:int);�rkez�s ideje 	(int:int)

szintaktikailag hib�s inputok eset�n a program hib�t dob
a feladatok v�grehajt�s�nak szimul�ci�ja id�tartamar�nyos (1 �ra 1 m�sodperc), azonban az �jonnan �rkez�
feladatokat nem lehet val�s id�ben megadni, csak a szimul�ci� el�tt, az utas�t�sokat k�vetve

mivel a feladat k�l�n nem t�rt ki erre, �j s�rg�s feladat �rkez�sekor, ha nem hajthat� v�gre a feladat f�l �r�n bel�l, mert �ppen egy m�sik s�rg�s feladat ker�l feldolgoz�sra, akkor kiv�telt gener�l a szoftver
ellenkez� esetben a feladatot f�lbeszak�tja �s �tt�r az �j s�rg�s feladat v�grehajt�s�ra
 
a szimul�ci�ban szerepl� id�pontok kiz�r�lag annak ellen�rz�s�t k�nny�tik meg, hogy a program 8 �r�ba osztja-e be
a feladatokat, az �resj�ratot, vagy a megszak�tott feladatokat nem veszi figyelembe
mivel a feladat a beoszt�s sorrendj�t tekinti, az �jonnan �rkezett feladatok a helyes sorrendben ker�lnek v�grehajt�sra, azonban  a megjelen�tett kezd�si idej�k nem felt�tlen�l val�s,  p�ld�ul ha kev�s, r�vid feladat ker�lt v�grehajt�sra a nap elej�n �s egy �j feladat �rkezik a nap v�g�n, akkor a szoftver �gy jelen�ti meg, mintha k�zvetlen�l a legutols� feladat ut�n kezdt�k volna meg annak v�grehajt�s�t akkor is, ha a megjelen�tett id�pont  kor�bbi mint a feladat meg�rkez�se

