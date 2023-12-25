
-- İlçe Tablosuna İlçe Eklendiğinde Ait Olduğu İldeki İlçe Sayısını Arttıran Trigger

CREATE OR REPLACE FUNCTION update_ilce_count()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Il
    SET IlceSayisi = IlceSayisi + 1
    WHERE IlId = NEW.IlId;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER ilce_insert_trigger
AFTER INSERT ON Ilce
FOR EACH ROW
EXECUTE FUNCTION update_ilce_count();


-- Şube Tablosuna Şube Eklendiğinde Ait Olduğu Bankadaki Şube Sayısını Arttıran Trigger

CREATE OR REPLACE FUNCTION sube_sayisi_guncelle()
RETURNS TRIGGER AS $$
BEGIN

    UPDATE Banka
    SET Sube_Sayisi = Sube_Sayisi + 1
    WHERE BankaID = NEW.BankaID;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_sube_sayisi_guncelle
AFTER INSERT ON Sube
FOR EACH ROW
EXECUTE FUNCTION sube_sayisi_guncelle();


-- Çalışan Tablosuna Çalışan Eklendikten Ait Olduğu Şubedeki Çalışan Sayısını Arttıran Trigger

CREATE OR REPLACE FUNCTION calisan_sayisi_guncelle()
RETURNS TRIGGER AS $$
BEGIN

    UPDATE Sube
    SET CalisanSayisi = CalisanSayisi + 1
    WHERE SubeID = NEW.SubeID;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_calisan_sayisi_guncelle
AFTER INSERT ON Calisan
FOR EACH ROW
EXECUTE FUNCTION calisan_sayisi_guncelle();


-- Para Transfer Tablosuna Veri Eklendikten Sonra İlgili Hesapların Bakiyesini Güncelleyen Ve
-- İşlem Tablosuna Her Hesap İçin Yapılan İşlemi Ekleyen Trigger

CREATE OR REPLACE FUNCTION add_transfer_and_update_balances() RETURNS TRIGGER AS $$
BEGIN

    INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih)
    VALUES (NEW.GonderenHesapId, (SELECT IslemTuruID FROM IslemTuru WHERE TurAdi = 'Para Gonderme'), NEW.Miktar, NEW.Tarih);

    INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih)
    VALUES (NEW.AliciHesapId, (SELECT IslemTuruID FROM IslemTuru WHERE TurAdi = 'Para Alma'), NEW.Miktar, NEW.Tarih);


    IF EXISTS (SELECT 1 FROM VadesizHesap WHERE HesapId = NEW.GonderenHesapId) THEN
        UPDATE VadesizHesap SET Bakiye = Bakiye - NEW.Miktar WHERE HesapId = NEW.GonderenHesapId;
    ELSE
        UPDATE VadeliHesap SET Bakiye = Bakiye - NEW.Miktar WHERE HesapId = NEW.GonderenHesapId;
    END IF;

    IF EXISTS (SELECT 1 FROM VadesizHesap WHERE HesapId = NEW.AliciHesapId) THEN
        UPDATE VadesizHesap SET Bakiye = Bakiye + NEW.Miktar WHERE HesapId = NEW.AliciHesapId;
    ELSE
        UPDATE VadeliHesap SET Bakiye = Bakiye + NEW.Miktar WHERE HesapId = NEW.AliciHesapId;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER add_transfer_and_update_balances_trigger
AFTER INSERT ON ParaTransferi
FOR EACH ROW EXECUTE PROCEDURE add_transfer_and_update_balances();


-- Döviz Hesabındaki Bir Hesap Güncellendikten Sonra İşlem Tablosuna Hesabın Yaptığı İşlemi Ekleyen Trigger

CREATE OR REPLACE FUNCTION doviz_islem_ekle() RETURNS TRIGGER AS $$
DECLARE
    doviz_alim_turu_id INT;
    doviz_satim_turu_id INT;
    islem_miktar DECIMAL;
BEGIN
 
    SELECT INTO doviz_alim_turu_id IslemTuruID FROM IslemTuru WHERE TurAdi = 'Doviz Alim';
    SELECT INTO doviz_satim_turu_id IslemTuruID FROM IslemTuru WHERE TurAdi = 'Doviz Satim';

 
    islem_miktar = NEW.Miktar - OLD.Miktar;


    IF islem_miktar > 0 THEN
        INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih) VALUES (NEW.DovizHesapID, doviz_alim_turu_id, islem_miktar, CURRENT_DATE);
  
    ELSE
        INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih) VALUES (NEW.DovizHesapID, doviz_satim_turu_id, -islem_miktar, CURRENT_DATE);
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER doviz_islem_ekle_trigger
AFTER UPDATE ON DovizHesap
FOR EACH ROW EXECUTE PROCEDURE doviz_islem_ekle();


-- Vadeli Ve Vadesiz Hesap Tablolarındaki Bir Hesap Güncellendikten Sonra İşlem Tablosuna Hesabın Yaptığı İşlemi Ekleyen Trigger

CREATE OR REPLACE FUNCTION islem_ekle() RETURNS TRIGGER AS $$
DECLARE
    para_yatirma_turu_id INT;
    para_cekme_turu_id INT;
    islem_miktar DECIMAL;
BEGIN
  
    SELECT INTO para_yatirma_turu_id IslemTuruID FROM IslemTuru WHERE TurAdi = 'Para Yatirma';
    SELECT INTO para_cekme_turu_id IslemTuruID FROM IslemTuru WHERE TurAdi = 'Para Cekme';

    islem_miktar = NEW.Bakiye - OLD.Bakiye;

   
    IF islem_miktar > 0 AND pg_trigger_depth() = 1 THEN
        INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih) VALUES (NEW.HesapID, para_yatirma_turu_id, islem_miktar, CURRENT_DATE);

    ELSEIF islem_miktar < 0 AND pg_trigger_depth() = 1 THEN
        INSERT INTO Islem (HesapID, IslemTuruID, Miktar, Tarih) VALUES (NEW.HesapID, para_cekme_turu_id, -islem_miktar, CURRENT_DATE);
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER islem_ekle_trigger
AFTER UPDATE OF Bakiye ON VadesizHesap
FOR EACH ROW EXECUTE PROCEDURE islem_ekle();

CREATE TRIGGER islem_ekle_trigger
AFTER UPDATE OF Bakiye ON VadeliHesap
FOR EACH ROW EXECUTE PROCEDURE islem_ekle();
