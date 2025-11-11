import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button, Badge, Table } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { firmaApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const FirmaList = () => {
  const [firmalar, setFirmalar] = useState([]);
  const [loading, setLoading] = useState(true);
  const { hasRole } = useAuth();

  useEffect(() => {
    loadFirmalar();
  }, []);

  const loadFirmalar = async () => {
    try {
      const response = await firmaApi.getAll();
      setFirmalar(response.data.data || []);
    } catch (error) {
      toast.error('Firmalar yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Bu firmayı silmek istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await firmaApi.delete(id);
      toast.success('Firma başarıyla silindi.');
      loadFirmalar();
    } catch (error) {
      toast.error('Firma silinirken hata oluştu.');
    }
  };

  const getFirmaTipiText = (tip) => {
    switch (tip) {
      case 1: return <Badge bg="primary">Müşteri</Badge>;
      case 2: return <Badge bg="success">Tedarikçi</Badge>;
      case 3: return <Badge bg="info">Her İkisi</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="warning">Beklemede</Badge>;
      case 2: return <Badge bg="success">Onaylandı</Badge>;
      case 3: return <Badge bg="danger">Reddedildi</Badge>;
      case 4: return <Badge bg="secondary">Pasif</Badge>;
      default: return '-';
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Yükleniyor...</span>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-building me-2"></i>
          Firma Yönetimi
        </h2>
        {(hasRole('Admin') || hasRole('Musteri')) && (
          <Link to="/firma/create" className="btn btn-primary">
            <i className="fas fa-plus me-2"></i>
            Yeni Firma Ekle
          </Link>
        )}
      </div>

      <div className="card shadow">
        <div className="card-body">
          <div className="table-responsive">
            <Table hover>
              <thead className="table-dark">
                <tr>
                  <th>Firma Adı</th>
                  <th>Vergi No</th>
                  <th>Email</th>
                  <th>Telefon</th>
                  <th>Firma Tipi</th>
                  <th>Durum</th>
                  <th>İşlemler</th>
                </tr>
              </thead>
              <tbody>
                {firmalar.length > 0 ? (
                  firmalar.map((firma) => (
                    <tr key={firma.id}>
                      <td><strong>{firma.ad}</strong></td>
                      <td>{firma.vergiNo}</td>
                      <td>{firma.email}</td>
                      <td>{firma.telefon}</td>
                      <td>{getFirmaTipiText(firma.firmaTipi)}</td>
                      <td>{getDurumText(firma.durum)}</td>
                      <td>
                        <Link to={`/firma/details/${firma.id}`} className="btn btn-sm btn-info me-1" title="Detaylar">
                          <i className="fas fa-eye"></i>
                        </Link>
                        <Link to={`/firma/edit/${firma.id}`} className="btn btn-sm btn-warning me-1" title="Düzenle">
                          <i className="fas fa-edit"></i>
                        </Link>
                        {hasRole('Admin') && (
                          <Button 
                            variant="danger" 
                            size="sm" 
                            onClick={() => handleDelete(firma.id)}
                            title="Sil"
                          >
                            <i className="fas fa-trash"></i>
                          </Button>
                        )}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="7" className="text-center text-muted py-4">
                      <i className="fas fa-info-circle me-2"></i>
                      Henüz firma kaydı bulunmamaktadır.
                    </td>
                  </tr>
                )}
              </tbody>
            </Table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default FirmaList;
