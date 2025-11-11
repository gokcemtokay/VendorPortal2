import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { firmaApi } from '../../api/apiClient';

const FirmaCreate = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    ad: '',
    vergiNo: '',
    email: '',
    telefon: '',
    adres: '',
    webSitesi: '',
    firmaTipi: '1'
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await firmaApi.create({ ...formData, firmaTipi: parseInt(formData.firmaTipi) });
      toast.success('Firma başarıyla oluşturuldu!');
      navigate('/firma');
    } catch (error) {
      toast.error('Firma oluşturulurken hata oluştu.');
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-md-8">
        <Card>
          <Card.Header className="bg-primary text-white">
            <h4><i className="fas fa-plus me-2"></i>Yeni Firma Ekle</h4>
          </Card.Header>
          <Card.Body>
            <Form onSubmit={handleSubmit}>
              <Form.Group className="mb-3">
                <Form.Label>Firma Adı *</Form.Label>
                <Form.Control
                  value={formData.ad}
                  onChange={(e) => setFormData({ ...formData, ad: e.target.value })}
                  required
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label>Vergi No</Form.Label>
                <Form.Control
                  value={formData.vergiNo}
                  onChange={(e) => setFormData({ ...formData, vergiNo: e.target.value })}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label>Email</Form.Label>
                <Form.Control
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
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
                <Button variant="primary" type="submit">
                  <i className="fas fa-save me-2"></i>Kaydet
                </Button>
                <Button variant="secondary" onClick={() => navigate('/firma')}>
                  <i className="fas fa-arrow-left me-2"></i>İptal
                </Button>
              </div>
            </Form>
          </Card.Body>
        </Card>
      </div>
    </div>
  );
};

export default FirmaCreate;
