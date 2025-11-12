import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button, Table, Badge, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ihaleApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const IhaleList = () => {
  const [ihaleler, setIhaleler] = useState([]);
  const [loading, setLoading] = useState(true);
  const { hasRole } = useAuth();

  useEffect(() => {
    loadIhaleler();
  }, []);

  const loadIhaleler = async () => {
    try {
      const response = await ihaleApi.getAll();
      setIhaleler(response.data.data || []);
    } catch (error) {
      toast.error('İhaleler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Bu ihaleyi silmek istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await ihaleApi.delete(id);
      toast.success('İhale başarıyla silindi.');
      loadIhaleler();
    } catch (error) {
      toast.error('İhale silinirken hata oluştu.');
    }
  };

  const handleYayinla = async (id) => {
    if (!window.confirm('Bu ihaleyi yayınlamak istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await ihaleApi.yayinla(id);
      toast.success('İhale başarıyla yayınlandı!');
      loadIhaleler();
    } catch (error) {
      toast.error('İhale yayınlanırken hata oluştu.');
    }
  };

  const getIhaleTuruText = (tur) => {
    switch (tur) {
      case 1: return <Badge bg="info">Herkese Açık</Badge>;
      case 2: return <Badge bg="warning">Davetli</Badge>;
      case 3: return <Badge bg="secondary">Kapalı</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="secondary">Taslak</Badge>;
      case 2: return <Badge bg="success">Yayında</Badge>;
      case 3: return <Badge bg="primary">Tamamlandı</Badge>;
      case 4: return <Badge bg="danger">İptal</Badge>;
      default: return '-';
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-gavel me-2"></i>
          İhale Yönetimi
        </h2>
        {(hasRole('Admin') || hasRole('Musteri')) && (
          <Link to="/ihale/create" className="btn btn-primary">
            <i className="fas fa-plus me-2"></i>
            Yeni İhale Aç
          </Link>
        )}
      </div>

      <div className="card shadow">
        <div className="card-body">
          {loading ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="primary" />
              <p className="mt-2">Yükleniyor...</p>
            </div>
          ) : (
            <div className="table-responsive">
              <Table hover>
                <thead className="table-dark">
                  <tr>
                    <th>İhale Adı</th>
                    <th>Referans No</th>
                    <th>Müşteri Firma</th>
                    <th>İhale Türü</th>
                    <th>Durum</th>
                    <th>Başlangıç</th>
                    <th>Bitiş</th>
                    <th>İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {ihaleler.length > 0 ? (
                    ihaleler.map((ihale) => (
                      <tr key={ihale.id}>
                        <td>
                          <strong>{ihale.ihaleAdi}</strong>
                        </td>
                        <td>{ihale.referansNo}</td>
                        <td>{ihale.musteriFirma?.ad || '-'}</td>
                        <td>{getIhaleTuruText(ihale.ihaleTuru)}</td>
                        <td>{getDurumText(ihale.durum)}</td>
                        <td>
                          {ihale.teklifBaslangicTarihi 
                            ? new Date(ihale.teklifBaslangicTarihi).toLocaleDateString('tr-TR')
                            : '-'}
                        </td>
                        <td>
                          {ihale.teklifBitisTarihi 
                            ? new Date(ihale.teklifBitisTarihi).toLocaleDateString('tr-TR')
                            : '-'}
                        </td>
                        <td>
                          <Link 
                            to={`/ihale/details/${ihale.id}`}
                            className="btn btn-sm btn-info me-2"
                            title="Detay"
                          >
                            <i className="fas fa-eye"></i>
                          </Link>
                          
                          {ihale.durum === 1 && (hasRole('Admin') || hasRole('Musteri')) && (
                            <>
                              <Link 
                                to={`/ihale/edit/${ihale.id}`}
                                className="btn btn-sm btn-warning me-2"
                                title="Düzenle"
                              >
                                <i className="fas fa-edit"></i>
                              </Link>
                              <Button
                                variant="success"
                                size="sm"
                                className="me-2"
                                onClick={() => handleYayinla(ihale.id)}
                                title="Yayınla"
                              >
                                <i className="fas fa-rocket"></i>
                              </Button>
                            </>
                          )}

                          {ihale.durum === 2 && (
                            <Link 
                              to={`/ihale/teklifler/${ihale.id}`}
                              className="btn btn-sm btn-primary me-2"
                              title="Teklifler"
                            >
                              <i className="fas fa-file-invoice-dollar"></i>
                            </Link>
                          )}

                          {hasRole('Admin') && (
                            <Button
                              variant="danger"
                              size="sm"
                              onClick={() => handleDelete(ihale.id)}
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
                      <td colSpan="8" className="text-center py-4">
                        <i className="fas fa-inbox fa-3x text-muted mb-3 d-block"></i>
                        Henüz ihale eklenmemiş.
                      </td>
                    </tr>
                  )}
                </tbody>
              </Table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default IhaleList;
