import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Button, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { malzemeApi, firmaApi } from '../../api/apiClient';

const MalzemeCreate = () => {
  const navigate = useNavigate();
  const [firmalar, setFirmalar] = useState([]);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    firmaId: '',
    kod: '',
    ad: '',
    aciklama: '',
    birim: 'Adet',
    fiyat: '',
    paraBirimi: 'TRY'
  });

  useEffect(() => {
    loadFirmalar();
  }, []);

  const loadFirmalar = async () => {
    try {
      const response = await firmaApi.getAll();
      setFirmalar(response.data.data || []);
    } catch (error) {
      toast.error('Firmalar yüklenirken hata oluştu.');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      const data = {
        ...formData,
        fiyat: formData.fiyat ? parseFloat(formData.fiyat) : null
      };
      
      await malzemeApi.create(data);
      toast.success('Malzeme başarıyla oluşturuldu!');
      navigate('/malzeme');
    } catch (error) {
      toast.error('Malzeme oluşturulurken hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <h2>
          <i className="fas fa-plus-circle me-2"></i>
          Yeni Malzeme Ekle
        </h2>
      </div>

      <Card className="shadow">
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Firma *</Form.Label>
              <Form.Select
                value={formData.firmaId}
                onChange={(e) => setFormData({ ...formData, firmaId: e.target.value })}
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
              <Form.Label>Malzeme Kodu *</Form.Label>
              <Form.Control
                type="text"
                value={formData.kod}
                onChange={(e) => setFormData({ ...formData, kod: e.target.value })}
                required
                placeholder="MLZ001"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Malzeme Adı *</Form.Label>
              <Form.Control
                type="text"
                value={formData.ad}
                onChange={(e) => setFormData({ ...formData, ad: e.target.value })}
                required
                placeholder="Malzeme adını giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Açıklama</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={formData.aciklama}
                onChange={(e) => setFormData({ ...formData, aciklama: e.target.value })}
                placeholder="Malzeme açıklaması (opsiyonel)"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Birim</Form.Label>
              <Form.Select
                value={formData.birim}
                onChange={(e) => setFormData({ ...formData, birim: e.target.value })}
              >
                <option value="Adet">Adet</option>
                <option value="KG">KG</option>
                <option value="Gram">Gram</option>
                <option value="Ton">Ton</option>
                <option value="Metre">Metre</option>
                <option value="Litre">Litre</option>
                <option value="M2">M²</option>
                <option value="M3">M³</option>
                <option value="Paket">Paket</option>
                <option value="Kutu">Kutu</option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Fiyat</Form.Label>
              <Form.Control
                type="number"
                step="0.01"
                value={formData.fiyat}
                onChange={(e) => setFormData({ ...formData, fiyat: e.target.value })}
                placeholder="0.00"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Para Birimi</Form.Label>
              <Form.Select
                value={formData.paraBirimi}
                onChange={(e) => setFormData({ ...formData, paraBirimi: e.target.value })}
              >
                <option value="TRY">TRY (₺)</option>
                <option value="USD">USD ($)</option>
                <option value="EUR">EUR (€)</option>
                <option value="GBP">GBP (£)</option>
              </Form.Select>
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
                    Kaydediliyor...
                  </>
                ) : (
                  <>
                    <i className="fas fa-save me-2"></i>
                    Kaydet
                  </>
                )}
              </Button>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/malzeme')}
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

export default MalzemeCreate;
