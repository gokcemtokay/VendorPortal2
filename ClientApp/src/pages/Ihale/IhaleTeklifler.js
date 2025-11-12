import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Card, Button, Table, Badge, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ihaleApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const IhaleTeklifler = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { hasRole } = useAuth();
  const [ihale, setIhale] = useState(null);
  const [teklifler, setTeklifler] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, [id]);

  const loadData = async () => {
    try {
      // İhale bilgilerini yükle
      const ihaleResponse = await ihaleApi.getById(id);
      setIhale(ihaleResponse.data.data);

      // Teklifleri yükle
      const teklifResponse = await ihaleApi.getTeklifler(id);
      setTeklifler(teklifResponse.data.data || []);
    } catch (error) {
      toast.error('Veriler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleOnaylaTeklif = async (teklifId) => {
    if (!window.confirm('Bu teklifi onaylamak istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await ihaleApi.onaylaTeklif(teklifId);
      toast.success('Teklif başarıyla onaylandı!');
      loadData();
    } catch (error) {
      toast.error('Teklif onaylanırken hata oluştu.');
    }
  };

  const getTeklifDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="warning">Beklemede</Badge>;
      case 2: return <Badge bg="success">Onaylandı</Badge>;
      case 3: return <Badge bg="danger">Reddedildi</Badge>;
      default: return '-';
    }
  };

  const calculateToplamTutar = (teklif) => {
    if (!teklif.kalemler || teklif.kalemler.length === 0) return 0;
    return teklif.kalemler.reduce((sum, kalem) => sum + (kalem.toplamFiyat || 0), 0);
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
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-file-invoice-dollar me-2"></i>
          İhale Teklifleri
        </h2>
        <Button 
          variant="secondary" 
          onClick={() => navigate(`/ihale/details/${id}`)}
        >
          <i className="fas fa-arrow-left me-2"></i>
          İhaleye Dön
        </Button>
      </div>

      {ihale && (
        <Card className="shadow mb-4">
          <Card.Header className="bg-primary text-white">
            <h5 className="mb-0">
              <i className="fas fa-gavel me-2"></i>
              {ihale.ihaleAdi} - {ihale.referansNo}
            </h5>
          </Card.Header>
          <Card.Body>
            <p className="mb-1"><strong>Müşteri:</strong> {ihale.musteriFirma?.ad}</p>
            <p className="mb-1">
              <strong>Teklif Bitiş:</strong> {' '}
              {ihale.teklifBitisTarihi 
                ? new Date(ihale.teklifBitisTarihi).toLocaleString('tr-TR')
                : '-'}
            </p>
            <p className="mb-0">
              <strong>Toplam Teklif Sayısı:</strong> {' '}
              <Badge bg="info">{teklifler.length}</Badge>
            </p>
          </Card.Body>
        </Card>
      )}

      <div className="card shadow">
        <div className="card-body">
          {teklifler.length > 0 ? (
            <div className="table-responsive">
              <Table hover>
                <thead className="table-dark">
                  <tr>
                    <th>Teklif No</th>
                    <th>Tedarikçi</th>
                    <th>Teklif Tarihi</th>
                    <th>Toplam Tutar</th>
                    <th>Para Birimi</th>
                    <th>Durum</th>
                    <th>İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {teklifler.map((teklif) => (
                    <tr key={teklif.id}>
                      <td>
                        <strong>{teklif.teklifNo}</strong>
                      </td>
                      <td>{teklif.tedarikciFirma?.ad || '-'}</td>
                      <td>
                        {teklif.teklifTarihi 
                          ? new Date(teklif.teklifTarihi).toLocaleString('tr-TR')
                          : '-'}
                      </td>
                      <td>
                        {calculateToplamTutar(teklif).toLocaleString('tr-TR', {
                          minimumFractionDigits: 2,
                          maximumFractionDigits: 2
                        })}
                      </td>
                      <td>
                        <Badge bg="secondary">{ihale?.paraBirimi || 'TRY'}</Badge>
                      </td>
                      <td>{getTeklifDurumText(teklif.durum)}</td>
                      <td>
                        {(hasRole('Admin') || hasRole('Musteri')) && teklif.durum === 1 && (
                          <Button
                            variant="success"
                            size="sm"
                            onClick={() => handleOnaylaTeklif(teklif.id)}
                          >
                            <i className="fas fa-check me-1"></i>
                            Onayla
                          </Button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          ) : (
            <div className="text-center py-5">
              <i className="fas fa-inbox fa-3x text-muted mb-3 d-block"></i>
              <p className="text-muted">Henüz teklif verilmemiş.</p>
            </div>
          )}
        </div>
      </div>

      {teklifler.length > 0 && (
        <Card className="shadow mt-4">
          <Card.Header className="bg-info text-white">
            <h5 className="mb-0">
              <i className="fas fa-chart-bar me-2"></i>
              Teklif Karşılaştırma
            </h5>
          </Card.Header>
          <Card.Body>
            <div className="row">
              <div className="col-md-4">
                <div className="card border-success">
                  <div className="card-body text-center">
                    <h6 className="text-success">En Düşük Teklif</h6>
                    <h3>
                      {Math.min(...teklifler.map(t => calculateToplamTutar(t))).toLocaleString('tr-TR', {
                        minimumFractionDigits: 2
                      })}
                      <small className="text-muted ms-2">{ihale?.paraBirimi}</small>
                    </h3>
                  </div>
                </div>
              </div>
              <div className="col-md-4">
                <div className="card border-info">
                  <div className="card-body text-center">
                    <h6 className="text-info">Ortalama Teklif</h6>
                    <h3>
                      {(teklifler.reduce((sum, t) => sum + calculateToplamTutar(t), 0) / teklifler.length).toLocaleString('tr-TR', {
                        minimumFractionDigits: 2
                      })}
                      <small className="text-muted ms-2">{ihale?.paraBirimi}</small>
                    </h3>
                  </div>
                </div>
              </div>
              <div className="col-md-4">
                <div className="card border-danger">
                  <div className="card-body text-center">
                    <h6 className="text-danger">En Yüksek Teklif</h6>
                    <h3>
                      {Math.max(...teklifler.map(t => calculateToplamTutar(t))).toLocaleString('tr-TR', {
                        minimumFractionDigits: 2
                      })}
                      <small className="text-muted ms-2">{ihale?.paraBirimi}</small>
                    </h3>
                  </div>
                </div>
              </div>
            </div>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default IhaleTeklifler;
