
-- İstenen Döviz Hesabının Değerini Hesaplayan Fonksiyon

CREATE OR REPLACE FUNCTION doviz_hesap_degeri(_doviz_hesap_id INT) RETURNS DECIMAL AS $$
DECLARE
    doviz_kuru DECIMAL;
    hesap_miktar DECIMAL;
BEGIN
    
    SELECT INTO doviz_kuru AlisKuru FROM Doviz WHERE DovizID = (SELECT DovizID FROM DovizHesap WHERE DovizHesapID = _doviz_hesap_id);
    SELECT INTO hesap_miktar Miktar FROM DovizHesap WHERE DovizHesapID = _doviz_hesap_id;


    RETURN doviz_kuru * hesap_miktar;
END;
$$ LANGUAGE plpgsql;


-- TC Kimlik Numarasına Göre Aranmak İstenen Müşterinin Bilgilerini Getiren Fonksiyon 

CREATE OR REPLACE FUNCTION get_musteri_info(p_tckn CHARACTER VARYING)
RETURNS TABLE(
    KisiId INT,
    Ad CHARACTER VARYING(40),
    Soyad CHARACTER VARYING(40),
    TCKN CHARACTER VARYING(11),
    DogumTarihi DATE,
    Cinsiyet CHARACTER(1),
    Telefon CHARACTER VARYING(20),
    Sifre CHARACTER VARYING(255),
    KisiTuru CHARACTER(1),
    MusteriTuru CHARACTER VARYING(40)
) AS $$
BEGIN
    RETURN QUERY 
    SELECT Kisi.KisiId, Kisi.Ad, Kisi.Soyad, Kisi.TCKN, Kisi.DogumTarihi, Kisi.Cinsiyet, Kisi.Telefon, Kisi.Sifre, Kisi.KisiTuru, Musteri.MusteriTuru
    FROM Kisi
    INNER JOIN Musteri ON Kisi.KisiId = Musteri.KisiId
    WHERE Kisi.TCKN = p_tckn;
END; $$
LANGUAGE 'plpgsql';

-- Döviz Türlerinin Birbirleri Cinsinden Miktarlarını Bulan Fonksiyon 

CREATE OR REPLACE FUNCTION doviz_donustur(p_baslangic_doviz_id INT, p_hedef_doviz_id INT, p_miktar NUMERIC)
RETURNS TABLE(
    DonusturulenDoviz CHARACTER VARYING,
    DonusturulenMiktar NUMERIC,
    DonusturulecekDoviz CHARACTER VARYING,
    DonusturulecekMiktar NUMERIC
) AS $$
DECLARE
    baslangic_kur NUMERIC;
    hedef_kur NUMERIC;
    baslangic_doviz_adi CHARACTER VARYING;
    hedef_doviz_adi CHARACTER VARYING;
BEGIN
    SELECT AlisKuru, DovizAdi INTO baslangic_kur, baslangic_doviz_adi FROM Doviz WHERE DovizId = p_baslangic_doviz_id;
    SELECT SatisKuru, DovizAdi INTO hedef_kur, hedef_doviz_adi FROM Doviz WHERE DovizId = p_hedef_doviz_id;

    DonusturulenDoviz := baslangic_doviz_adi;
    DonusturulenMiktar := p_miktar;
    DonusturulecekDoviz := hedef_doviz_adi;
    DonusturulecekMiktar := (p_miktar * baslangic_kur) / hedef_kur;

    RETURN NEXT;
END; $$
LANGUAGE 'plpgsql';


-- İstenen Müşterinin Kredi Bilgilerini Getiren Fonksiyon

CREATE OR REPLACE FUNCTION get_kredi_info(musteri_id INT)
RETURNS TABLE(
    KrediID INT,
    Ad CHARACTER VARYING(40),
    Soyad CHARACTER VARYING(40),
    TCKN CHARACTER VARYING(11),
    HesapNumarasi VARCHAR(20),
    KrediTuru VARCHAR(255),
    Miktar DECIMAL(10, 2),
    BaslamaTarihi DATE,
    BitisTarihi DATE
) AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Kredi 
                   JOIN Hesap ON Kredi.HesapId = Hesap.HesapId 
                   WHERE Hesap.MusteriId = musteri_id) THEN
        RAISE EXCEPTION 'Müşterinin kredisi bulunamadı';
    END IF;
    RETURN QUERY 
    SELECT Kredi.KrediID, Kisi.Ad, Kisi.Soyad, Kisi.TCKN, Hesap.HesapNumarasi, KrediCesitleri.KrediTuru, Kredi.Miktar, Kredi.BaslamaTarihi, Kredi.BitisTarihi
    FROM Kredi
    JOIN Hesap ON Kredi.HesapId = Hesap.HesapId
    JOIN KrediCesitleri ON Kredi.KrediCesitleriId = KrediCesitleri.KrediCesitleriID
    JOIN Musteri ON Hesap.MusteriId = Musteri.KisiId
    JOIN Kisi ON Musteri.KisiId = Kisi.KisiId
    WHERE Hesap.MusteriId = musteri_id;
END; $$
LANGUAGE plpgsql;


-- İstenen Müşterinin İşlem Geçmişini Getiren Fonksiyon

CREATE OR REPLACE FUNCTION get_islem_info(musteri_id INT)
RETURNS TABLE(
    Ad CHARACTER VARYING(40),
    Soyad CHARACTER VARYING(40),
    HesapNumarasi VARCHAR(20),
    TurAdi VARCHAR(255),
    Miktar DECIMAL(10, 2),
    Tarih DATE
) AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Islem 
                   JOIN Hesap ON Islem.HesapID = Hesap.HesapId 
                   WHERE Hesap.MusteriId = musteri_id) THEN
        RAISE EXCEPTION 'Müşterinin işlemi bulunamadı';
    END IF;

    RETURN QUERY 
    SELECT Kisi.Ad, Kisi.Soyad, Hesap.HesapNumarasi, IslemTuru.TurAdi, Islem.Miktar, Islem.Tarih
    FROM Islem
    JOIN Hesap ON Islem.HesapID = Hesap.HesapId
    JOIN IslemTuru ON Islem.IslemTuruID = IslemTuru.IslemTuruID
    JOIN Musteri ON Hesap.MusteriId = Musteri.KisiId
    JOIN Kisi ON Musteri.KisiId = Kisi.KisiId
    WHERE Hesap.MusteriId = musteri_id;
END; $$
LANGUAGE plpgsql;
