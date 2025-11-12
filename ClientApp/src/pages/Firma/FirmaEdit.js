import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Card, Form, Button, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { firmaApi } from '../../api/apiClient';

const FirmaEdit = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    id: '',
    ad: '',
    vergiNo: '',
    email: '',
    telefon: '',
    adres: '',
    webSitesi: '',
    firmaTipi: '1'
  });

  useEffect(() => {
    loadFirma();
  }, [id]);

  const loadFirma = async () => {
    try {
      const response = await firmaApi.getById(id);
      const firma = response.data.data;
      setFormData({
        id: firma.id,
        ad: firma.ad || '',
        vergiNo: firma.vergiNo || '',
        email: firma.email || '',
        telefon: firma.telefon || '',
        adres: firma.adres || '',
        webSitesi: firma.webSitesi || '',
        firmaTipi: firma.firmaTipi?.toString() || '1'
      });
    } catch (error) {
      toast.error('Firma bilgileri yüklenirken hata oluştu.');
      navigate('/firma');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    
    try {
      await firmaApi.update({ 
        ...formData, 
        firmaTipi: parseInt(formData.firmaTipi) 
      });
      toast.success('Firma başarıyla güncellendi!');
      navigate('/firma');
    } catch (error) {
      toast.error('Firma güncellenirken hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <Spinner animation="border" variant="primary" />
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4">
        <h2>
          <i className="fas fa-edit me-2"></i>
          Firma Düzenle
        </h2>
      </div>

      <Card className="shadow">
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Firma Adı *</Form.Label>
              <Form.Control
                type="text"
                value={formData.ad}
                onChange={(e) => setFormData({ ...formData, ad: e.target.value })}
                required
                placeholder="Firma adını giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Vergi No</Form.Label>
              <Form.Control
                type="text"
                value={formData.vergiNo}
                onChange={(e) => setFormData({ ...formData, vergiNo: e.target.value })}
                placeholder="Vergi numarasını giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                placeholder="Email adresini giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Telefon</Form.Label>
              <Form.Control
                type="tel"
                value={formData.telefon}
                onChange={(e) => setFormData({ ...formData, telefon: e.target.value })}
                placeholder="Telefon numarasını giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Adres</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={formData.adres}
                onChange={(e) => setFormData({ ...formData, adres: e.target.value })}
                placeholder="Adresi giriniz"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Web Sitesi</Form.Label>
              <Form.Control
                type="url"
                value={formData.webSitesi}
                onChange={(e) => setFormData({ ...formData, webSitesi: e.target.value })}
                placeholder="https://www.ornek.com"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Firma Tipi *</Form.Label>
              <Form.Select
                value={formData.firmaTipi}
                onChange={(e) => setFormData({ ...formData, firmaTipi: e.target.value })}
                required
              >
                <option value="1">Müşteri</option>
                <option value="2">Tedarikçi</option>
                <option value="3">Her İkisi</option>
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
                    Güncelleniyor...
                  </>
                ) : (
                  <>
                    <i className="fas fa-save me-2"></i>
                    Güncelle
                  </>
                )}
              </Button>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/firma')}
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

export default FirmaEdit;
