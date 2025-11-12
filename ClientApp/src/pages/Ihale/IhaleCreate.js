import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Button, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ihaleApi, firmaApi } from '../../api/apiClient';

const IhaleCreate = () => {
  const navigate = useNavigate();
  const [firmalar, setFirmalar] = useState([]);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    musteriFirmaId: '',
    ihaleAdi: '',
    referansNo: '',
    aciklama: '',
    ihaleTuru: '1',
    teklifBaslangicTarihi: '',
    teklifBitisTarihi: '',
    gecerlilikSuresiGun: 30,
    paraBirimi: 'TRY',
    herkeseAcikMi: true,
    tumTedarikcilereAcikMi: false
  });

  useEffect(() => {
    loadFirmalar();
    generateReferansNo();
  }, []);

  const loadFirmalar = async () => {
    try {
      const response = await firmaApi.getAll();
      // Sadece müşteri firmalarını filtrele
      const musteriFirmalar = response.data.data?.filter(f => f.firmaTipi === 1 || f.firmaTipi === 3) || [];
      setFirmalar(musteriFirmalar);
    } catch (error) {
      toast.error('Firmalar yüklenirken hata oluştu.');
    }
  };

  const generateReferansNo = () => {
    const random = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
    const referansNo = `IHL${new Date().getFullYear()}${random}`;
    setFormData(prev => ({ ...prev, referansNo }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const ihaleData = {
        ...formData,
        ihaleTuru: parseInt(formData.ihaleTuru),
        gecerlilikSuresiGun: parseInt(formData.gecerlilikSuresiGun)
      };

      await ihaleApi.create({
        ihale: ihaleData,
        kalemler: [] // Kalemler sonra eklenebilir
      });
      
      toast.success('İhale başarıyla oluşturuldu!');
      navigate('/ihale');
    } catch (error) {
      toast.error('İhale oluşturulurken hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <h2>
          <i className="fas fa-plus-circle me-2"></i>
          Yeni İhale Aç
        </h2>
      </div>

      <Card className="shadow">
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Müşteri Firma *</Form.Label>
              <Form.Select
                value={formData.musteriFirmaId}
                onChange={(e) => setFormData({ ...formData, musteriFirmaId: e.target.value })}
                required
              >
                <option value="">Firma seçiniz...</option>
                {firmalar.map(firma => (
                  <option key={firma.id} value={firma.id}>
                    {firma.ad}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>İhale Adı *</Form.Label>
              <Form.Control
                type="text"
                value={formData.ihaleAdi}
                onChange={(e) => setFormData({ ...formData, ihaleAdi: e.target.value })}
                required
                placeholder="İhale başlığını giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Referans No *</Form.Label>
              <Form.Control
                type="text"
                value={formData.referansNo}
                onChange={(e) => setFormData({ ...formData, referansNo: e.target.value })}
                required
                placeholder="IHL20240001"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Açıklama</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={formData.aciklama}
                onChange={(e) => setFormData({ ...formData, aciklama: e.target.value })}
                placeholder="İhale açıklaması (opsiyonel)"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>İhale Türü *</Form.Label>
              <Form.Select
                value={formData.ihaleTuru}
                onChange={(e) => setFormData({ ...formData, ihaleTuru: e.target.value })}
                required
              >
                <option value="1">Herkese Açık</option>
                <option value="2">Davetli</option>
                <option value="3">Kapalı</option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Teklif Başlangıç Tarihi *</Form.Label>
              <Form.Control
                type="datetime-local"
                value={formData.teklifBaslangicTarihi}
                onChange={(e) => setFormData({ ...formData, teklifBaslangicTarihi: e.target.value })}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Teklif Bitiş Tarihi *</Form.Label>
              <Form.Control
                type="datetime-local"
                value={formData.teklifBitisTarihi}
                onChange={(e) => setFormData({ ...formData, teklifBitisTarihi: e.target.value })}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Geçerlilik Süresi (Gün)</Form.Label>
              <Form.Control
                type="number"
                value={formData.gecerlilikSuresiGun}
                onChange={(e) => setFormData({ ...formData, gecerlilikSuresiGun: e.target.value })}
                placeholder="30"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Para Birimi *</Form.Label>
              <Form.Select
                value={formData.paraBirimi}
                onChange={(e) => setFormData({ ...formData, paraBirimi: e.target.value })}
                required
              >
                <option value="TRY">TRY (₺)</option>
                <option value="USD">USD ($)</option>
                <option value="EUR">EUR (€)</option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Check
                type="checkbox"
                label="Herkese Açık"
                checked={formData.herkeseAcikMi}
                onChange={(e) => setFormData({ ...formData, herkeseAcikMi: e.target.checked })}
              />
            </Form.Group>

            <div className="d-grid gap-2">
              <Button 
                variant="primary" 
                type="submit"
                disabled={saving}
              >
                {saving ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Oluşturuluyor...
                  </>
                ) : (
                  <>
                    <i className="fas fa-save me-2"></i>
                    İhale Oluştur
                  </>
                )}
              </Button>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/ihale')}
                disabled={saving}
              >
                <i className="fas fa-arrow-left me-2"></i>
                İptal
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </div>
  );
};

export default IhaleCreate;
