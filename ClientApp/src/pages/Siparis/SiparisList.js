import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button, Table, Badge, Form, Spinner, Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { siparisApi, firmaApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const SiparisList = () => {
  const [siparisler, setSiparisler] = useState([]);
  const [firmalar, setFirmalar] = useState([]);
  const [selectedFirmaId, setSelectedFirmaId] = useState('');
  const [isMusteriView, setIsMusteriView] = useState(true);
  const [loading, setLoading] = useState(false);
  const { hasRole, user } = useAuth();

  useEffect(() => {
    loadFirmalar();
  }, []);

  useEffect(() => {
    if (selectedFirmaId) {
      loadSiparisler();
    }
  }, [selectedFirmaId, isMusteriView]);

  const loadFirmalar = async () => {
    try {
      const response = await firmaApi.getAll();
      setFirmalar(response.data.data || []);
    } catch (error) {
      console.error('Firmalar yüklenirken hata:', error);
    }
  };

  const loadSiparisler = async () => {
    if (!selectedFirmaId) return;
    
    setLoading(true);
    try {
      const response = await siparisApi.getSiparislerByFirma(selectedFirmaId, isMusteriView);
      setSiparisler(response.data.data || []);
    } catch (error) {
      toast.error('Siparişler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const getSiparisTuruText = (tur) => {
    switch (tur) {
      case 1: return <Badge bg="primary">Satın Alma</Badge>;
      case 2: return <Badge bg="success">Satış</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="secondary">Oluşturuldu</Badge>;
      case 2: return <Badge bg="warning">Bekliyor</Badge>;
      case 3: return <Badge bg="success">Onaylandı</Badge>;
      case 4: return <Badge bg="info">İşleniyor</Badge>;
      case 5: return <Badge bg="primary">Tamamlandı</Badge>;
      case 6: return <Badge bg="danger">İptal</Badge>;
      default: return '-';
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-shopping-cart me-2"></i>
          Sipariş Yönetimi
        </h2>
      </div>

      <div className="card shadow mb-3">
        <div className="card-body">
          <Form.Group className="mb-3">
            <Form.Label>Firma Seçiniz</Form.Label>
            <Form.Select
              value={selectedFirmaId}
              onChange={(e) => setSelectedFirmaId(e.target.value)}
            >
              <option value="">Firma seçiniz...</option>
              {firmalar.map(firma => (
                <option key={firma.id} value={firma.id}>
                  {firma.ad}
                </option>
              ))}
            </Form.Select>
          </Form.Group>

          {selectedFirmaId && (
            <Tabs
              activeKey={isMusteriView ? 'musteri' : 'tedarikci'}
              onSelect={(k) => setIsMusteriView(k === 'musteri')}
              className="mb-3"
            >
              <Tab eventKey="musteri" title="Satın Alma Siparişleri">
              </Tab>
              <Tab eventKey="tedarikci" title="Satış Siparişleri">
              </Tab>
            </Tabs>
          )}
        </div>
      </div>

      {!selectedFirmaId ? (
        <div className="card shadow">
          <div className="card-body text-center py-5">
            <i className="fas fa-building fa-3x text-muted mb-3 d-block"></i>
            <p className="text-muted">Lütfen önce bir firma seçiniz.</p>
          </div>
        </div>
      ) : loading ? (
        <div className="card shadow">
          <div className="card-body text-center py-5">
            <Spinner animation="border" variant="primary" />
            <p className="mt-2">Yükleniyor...</p>
          </div>
        </div>
      ) : (
        <div className="card shadow">
          <div className="card-body">
            <div className="table-responsive">
              <Table hover>
                <thead className="table-dark">
                  <tr>
                    <th>Sipariş No</th>
                    <th>Tarih</th>
                    <th>{isMusteriView ? 'Tedarikçi' : 'Müşteri'}</th>
                    <th>Sipariş Türü</th>
                    <th>Toplam Tutar</th>
                    <th>Para Birimi</th>
                    <th>Durum</th>
                    <th>İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {siparisler.length > 0 ? (
                    siparisler.map((siparis) => (
                      <tr key={siparis.id}>
                        <td>
                          <strong>{siparis.referansNo}</strong>
                        </td>
                        <td>
                          {siparis.siparisTarihi 
                            ? new Date(siparis.siparisTarihi).toLocaleDateString('tr-TR')
                            : '-'}
                        </td>
                        <td>
                          {isMusteriView 
                            ? siparis.tedarikciFirma?.ad 
                            : siparis.musteriFirma?.ad}
                        </td>
                        <td>{getSiparisTuruText(siparis.siparisTuru)}</td>
                        <td>
                          {siparis.toplamTutar 
                            ? siparis.toplamTutar.toLocaleString('tr-TR', { 
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2 
                              })
                            : '0,00'}
                        </td>
                        <td>
                          <Badge bg="secondary">{siparis.paraBirimi}</Badge>
                        </td>
                        <td>{getDurumText(siparis.durum)}</td>
                        <td>
                          <Link 
                            to={`/siparis/details/${siparis.id}`}
                            className="btn btn-sm btn-info"
                          >
                            <i className="fas fa-eye"></i> Detay
                          </Link>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="8" className="text-center py-4">
                        <i className="fas fa-inbox fa-3x text-muted mb-3 d-block"></i>
                        Henüz sipariş bulunmamaktadır.
                      </td>
                    </tr>
                  )}
                </tbody>
              </Table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default SiparisList;
