
-- İstenen Döviz Hesabı İçin Döviz Alımı Yapan Prosedür

CREATE OR REPLACE PROCEDURE doviz_alim(doviz_hesap_id INT, alim_miktar DECIMAL)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE DovizHesap SET Miktar = Miktar + alim_miktar WHERE DovizHesapID = doviz_hesap_id;
    UPDATE Doviz SET Rezerv = Rezerv - alim_miktar WHERE DovizID = (SELECT DovizID FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id);

    -- Miktar ve rezervin negatif olup olmadığını kontrolü
    IF (SELECT Miktar FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id) < 0 OR (SELECT Rezerv FROM Doviz WHERE DovizID = (SELECT DovizID FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id)) < 0 THEN
        RAISE EXCEPTION 'Miktar veya rezerv negatif olamaz';
    END IF;
END; $$;

-- İstenen Döviz Hesabı İçin Döviz Satımı Yapan Prosedür

CREATE OR REPLACE PROCEDURE doviz_satim(doviz_hesap_id INT, satim_miktar DECIMAL)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE DovizHesap SET Miktar = Miktar - satim_miktar WHERE DovizHesapID = doviz_hesap_id;
    UPDATE Doviz SET Rezerv = Rezerv + satim_miktar WHERE DovizID = (SELECT DovizID FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id);

    -- Miktar ve rezervin negatif olup olmadığını kontrolü
    IF (SELECT Miktar FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id) < 0 OR (SELECT Rezerv FROM Doviz WHERE DovizID = (SELECT DovizID FROM DovizHesap WHERE DovizHesapID = doviz_hesap_id)) < 0 THEN
        RAISE EXCEPTION 'Miktar veya rezerv negatif olamaz';
    END IF;
END; $$;

-- İki Hesap Arasında Para Gönderimi Bilgisini ParaTransferi Tablosuna Ekleyen Prosedür

CREATE OR REPLACE PROCEDURE transfer_money(_gonderen INT, _alici INT, _miktar DECIMAL)
LANGUAGE plpgsql
AS $$
BEGIN
    
    INSERT INTO ParaTransferi (GonderenHesapId, AliciHesapId, Miktar, Tarih) VALUES (_gonderen, _alici, _miktar, CURRENT_DATE);
END; $$;

-- İstenen Vadeli Veya Vadesiz Hesaba Para Yatırma İşlemi Yapan Prosedür

CREATE OR REPLACE PROCEDURE para_yatir(hesap_id INT, yatirilan_miktar DECIMAL)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Vadesiz hesap kontrolü
    IF EXISTS (SELECT 1 FROM VadesizHesap WHERE HesapId = hesap_id) THEN
        UPDATE VadesizHesap SET Bakiye = Bakiye + yatirilan_miktar WHERE HesapId = hesap_id;
    -- Vadeli hesap kontrolü
    ELSIF EXISTS (SELECT 1 FROM VadeliHesap WHERE HesapId = hesap_id) THEN
        UPDATE VadeliHesap SET Bakiye = Bakiye + yatirilan_miktar WHERE HesapId = hesap_id;
    ELSE
        RAISE EXCEPTION 'Hesap bulunamadı';
    END IF;
END; $$;

-- İstenen Vadeli Veya Vadesiz Hesaptan Para Çekme İşlemi Yapan Prosedür

CREATE OR REPLACE PROCEDURE para_cek(hesap_id INT, cekilen_miktar DECIMAL)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Vadesiz hesap kontrolü
    IF EXISTS (SELECT 1 FROM VadesizHesap WHERE HesapId = hesap_id) THEN
        UPDATE VadesizHesap SET Bakiye = Bakiye - cekilen_miktar WHERE HesapId = hesap_id;
        -- Bakiyenin negatif olup olmadığını kontrolü
        IF (SELECT Bakiye FROM VadesizHesap WHERE HesapId = hesap_id) < 0 THEN
            RAISE EXCEPTION 'Bakiye negatif olamaz';
        END IF;
    -- Vadeli hesap kontrolü
    ELSIF EXISTS (SELECT 1 FROM VadeliHesap WHERE HesapId = hesap_id) THEN
        UPDATE VadeliHesap SET Bakiye = Bakiye - cekilen_miktar WHERE HesapId = hesap_id;
        -- Bakiyenin negatif olup olmadığını kontrolü
        IF (SELECT Bakiye FROM VadeliHesap WHERE HesapId = hesap_id) < 0 THEN
            RAISE EXCEPTION 'Bakiye negatif olamaz';
        END IF;
    ELSE
        RAISE EXCEPTION 'Hesap bulunamadı';
    END IF;
END; $$;

-- Döviz Hesabı Ekleyen Prosedür

CREATE OR REPLACE PROCEDURE ekle_doviz_hesap(
    p_musteriid INT, 
    p_hesapnumarasi VARCHAR, 
    p_olusturulmatarihi DATE, 
    p_hesapturu VARCHAR, 
    p_miktar DECIMAL, 
    p_dovizid INT
) 
LANGUAGE plpgsql 
AS $$
DECLARE 
    v_hesapid INT;
BEGIN
    INSERT INTO Hesap (MusteriId, HesapNumarasi, OlusturulmaTarihi, HesapTuru) 
    VALUES (p_musteriid, p_hesapnumarasi, p_olusturulmatarihi, p_hesapturu)
    RETURNING HesapId INTO v_hesapid;

    INSERT INTO DovizHesap (DovizHesapID, Miktar, DovizID) 
    VALUES (v_hesapid, p_miktar, p_dovizid);
END; $$;
