CREATE TABLE Banka (
    BankaID SERIAL PRIMARY KEY,
    Ad VARCHAR(255) NOT NULL,
    Telefon VARCHAR(20) UNIQUE,
    Sube_Sayisi INT
);

CREATE TABLE Il (
    IlId SERIAL PRIMARY KEY,
    IlAd CHARACTER VARYING(50) NOT NULL UNIQUE,
    IlceSayisi INT NOT NULL
);

CREATE TABLE Ilce (
    IlceId SERIAL PRIMARY KEY,
    IlceAd CHARACTER VARYING(50) NOT NULL,
    IlId INT,
    CONSTRAINT fk_Ilce_Il FOREIGN KEY (IlId) REFERENCES Il(IlId)                    
);

CREATE TABLE Sube (
    SubeID SERIAL PRIMARY KEY,
    Ad VARCHAR(255) NOT NULL,
    Telefon VARCHAR(20) UNIQUE,
    IlId INT NOT NULL,
    IlceId INT NOT NULL,
    BankaID INT NOT NULL,
    CalisanSayisi INT NOT NULL,
    FOREIGN KEY (IlId) REFERENCES Il(IlId),
    FOREIGN KEY (IlceId) REFERENCES Ilce(IlceId),
    FOREIGN KEY (BankaID) REFERENCES Banka(BankaID)
);

CREATE TABLE Kasa (
    KasaID SERIAL PRIMARY KEY,
    SubeID INT,
    ToplamAnaPara DECIMAL(10, 2),	
    FOREIGN KEY (SubeID) REFERENCES Sube(SubeID)
);



CREATE TABLE Kisi ( 
    KisiId SERIAL,
    Ad CHARACTER VARYING(40) NOT NULL,
    Soyad CHARACTER VARYING(40) NOT NULL,
    TCKN CHARACTER VARYING(11) NOT NULL UNIQUE,
    DogumTarihi DATE NOT NULL,
    Cinsiyet CHARACTER(1) NOT NULL,
    Telefon CHARACTER VARYING(20) NOT NULL UNIQUE,
    Sifre CHARACTER VARYING(255) NOT NULL,
    KisiTuru CHARACTER(1) NOT NULL,
    CONSTRAINT KisiPK PRIMARY KEY (KisiId)
);

CREATE TABLE Musteri ( 
    KisiId INT,
    MusteriTuru CHARACTER VARYING(40) NOT NULL,
    CONSTRAINT musteriPK PRIMARY KEY (KisiId)
);

ALTER TABLE Musteri
    ADD CONSTRAINT MusteriKisi FOREIGN KEY (KisiId)
    REFERENCES Kisi (KisiId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;
	
CREATE TABLE Calisan (
    KisiId INT,
    Pozisyon CHARACTER VARYING(40) NOT NULL,
    BaslamaTarihi DATE NOT NULL,
    SubeID INT,
    CONSTRAINT calisanPK PRIMARY KEY (KisiId)
);

ALTER TABLE Calisan
    ADD CONSTRAINT CalisanKisi FOREIGN KEY (KisiId)
    REFERENCES Kisi (KisiId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;

ALTER TABLE Calisan
    ADD CONSTRAINT CalisanSube FOREIGN KEY (SubeID)
    REFERENCES Sube (SubeID)
    ON DELETE CASCADE
    ON UPDATE CASCADE;

CREATE TABLE Hesap (
    HesapId SERIAL PRIMARY KEY,
    MusteriId INT NOT NULL,
    HesapNumarasi VARCHAR(20) UNIQUE,
    OlusturulmaTarihi DATE NOT NULL,
    HesapTuru VARCHAR(40) NOT NULL,
    FOREIGN KEY (MusteriId) REFERENCES Musteri(KisiId)
);

CREATE TABLE VadesizHesap ( 
    HesapId INT,
    Bakiye DECIMAL(10, 2),
    CONSTRAINT vadesizPK PRIMARY KEY (HesapId)
);

ALTER TABLE VadesizHesap
    ADD CONSTRAINT Vadesiz FOREIGN KEY (HesapId)
    REFERENCES Hesap (HesapId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;
	
CREATE TABLE VadeliHesap ( 
    HesapId INT PRIMARY KEY,
    Bakiye DECIMAL(10, 2) NOT NULL,
    VadeTarihi DATE NOT NULL,
    FaizOrani DECIMAL(5, 2) NOT NULL
);

ALTER TABLE VadeliHesap
    ADD CONSTRAINT Vadeli FOREIGN KEY (HesapId)
    REFERENCES Hesap (HesapId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;
	
CREATE TABLE Doviz (
    DovizID SERIAL PRIMARY KEY,
    DovizAdi VARCHAR(50) NOT NULL UNIQUE,
    AlisKuru DECIMAL(10, 2) NOT NULL,
    SatisKuru DECIMAL(10, 2) NOT NULL,
    Rezerv DECIMAL(10, 2) NOT NULL
);

CREATE TABLE DovizHesap (
    DovizHesapID INT,
    Miktar DECIMAL(10, 2) NOT NULL,
    DovizID INT,
    CONSTRAINT DovizHesapPK PRIMARY KEY (DovizHesapID)
);

ALTER TABLE DovizHesap
    ADD CONSTRAINT DovizFK FOREIGN KEY (DovizID)
    REFERENCES Doviz (DovizId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;

ALTER TABLE DovizHesap
    ADD CONSTRAINT Doviz FOREIGN KEY (DovizID)
    REFERENCES Hesap (HesapId)
    ON DELETE CASCADE
    ON UPDATE CASCADE;

CREATE TABLE ParaTransferi (
    ParaTransferiId SERIAL PRIMARY KEY,
    GonderenHesapId INT NOT NULL,
    AliciHesapId INT NOT NULL,
    Miktar DECIMAL(10, 2) NOT NULL,
    Tarih DATE NOT NULL,
    FOREIGN KEY (GonderenHesapId) REFERENCES Hesap(HesapId),
    FOREIGN KEY (AliciHesapId) REFERENCES Hesap(HesapId)
);

CREATE TABLE IslemTuru (
    IslemTuruID SERIAL PRIMARY KEY,
    TurAdi VARCHAR(255) NOT NULL UNIQUE
);


CREATE TABLE Islem (
    IslemID SERIAL PRIMARY KEY,
    HesapID INT NOT NULL,
    IslemTuruID INT NOT NULL,
    Miktar DECIMAL(10, 2) NOT NULL,
    Tarih DATE NOT NULL,
    FOREIGN KEY (HesapID) REFERENCES Hesap(HesapId),
    FOREIGN KEY (IslemTuruID) REFERENCES IslemTuru(IslemTuruID)
);

CREATE TABLE KrediCesitleri (
    KrediCesitleriID SERIAL PRIMARY KEY,
    KrediTuru VARCHAR(255) NOT NULL UNIQUE,
    FaizOrani DECIMAL(5, 2) NOT NULL
);

CREATE TABLE Kart (
    KartID SERIAL PRIMARY KEY,
    HesapID INT NOT NULL,
    KartNo VARCHAR(20) UNIQUE,
    SonKullanmaTarihi DATE NOT NULL,
    KartTipi VARCHAR(40) NOT NULL,
    FOREIGN KEY (HesapID) REFERENCES Hesap(HesapId)
);

CREATE TABLE Kredi (
    KrediID SERIAL PRIMARY KEY,
    HesapId INT NOT NULL,
    KrediCesitleriId INT NOT NULL,
    Miktar DECIMAL(10, 2) NOT NULL,
    BaslamaTarihi DATE NOT NULL,
    BitisTarihi DATE NOT NULL,
    FOREIGN KEY (HesapId) REFERENCES Hesap(HesapId),
    FOREIGN KEY (KrediCesitleriId) REFERENCES KrediCesitleri(KrediCesitleriID)
);
