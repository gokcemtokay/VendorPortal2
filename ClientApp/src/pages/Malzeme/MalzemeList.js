import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button, Table, Badge, Form, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { malzemeApi, firmaApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const MalzemeList = () => {
  const [malzemeler, setMalzemeler] = useState([]);
  const [firmalar, setFirmalar] = useState([]);
  const [selectedFirmaId, setSelectedFirmaId] = useState('');
  const [loading, setLoading] = useState(true);
  const { hasRole } = useAuth();

  useEffect(() => {
    loadFirmalar();
  }, []);

  useEffect(() => {
    if (selectedFirmaId) {
      loadMalzemelerByFirma(selectedFirmaId);
    } else if (hasRole('Admin')) {
      loadAllMalzemeler();
    }
  }, [selectedFirmaId]);

  const loadFirmalar = async () => {
    try {
      const response = await firmaApi.getAll();
      setFirmalar(response.data.data || []);
    } catch (error) {
      console.error('Firmalar yüklenirken hata:', error);
    }
  };

  const loadAllMalzemeler = async () => {
    setLoading(true);
    try {
      const response = await malzemeApi.getAll();
      setMalzemeler(response.data.data || []);
    } catch (error) {
      toast.error('Malzemeler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const loadMalzemelerByFirma = async (firmaId) => {
    setLoading(true);
    try {
      const response = await malzemeApi.getByFirma(firmaId);
      setMalzemeler(response.data.data || []);
    } catch (error) {
      toast.error('Malzemeler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Bu malzemeyi silmek istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await malzemeApi.delete(id);
      toast.success('Malzeme başarıyla silindi.');
      if (selectedFirmaId) {
        loadMalzemelerByFirma(selectedFirmaId);
      } else {
        loadAllMalzemeler();
      }
    } catch (error) {
      toast.error('Malzeme silinirken hata oluştu.');
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-box me-2"></i>
          Malzeme Yönetimi
        </h2>
        <Link to="/malzeme/create" className="btn btn-primary">
          <i className="fas fa-plus me-2"></i>
          Yeni Malzeme Ekle
        </Link>
      </div>

      <div className="card shadow mb-3">
        <div className="card-body">
          <Form.Group>
            <Form.Label>Firma Seçiniz</Form.Label>
            <Form.Select
              value={selectedFirmaId}
              onChange={(e) => setSelectedFirmaId(e.target.value)}
            >
              <option value="">Tüm Firmalar</option>
              {firmalar.map(firma => (
                <option key={firma.id} value={firma.id}>
                  {firma.ad}
                </option>
              ))}
            </Form.Select>
          </Form.Group>
        </div>
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
                    <th>Malzeme Kodu</th>
                    <th>Malzeme Adı</th>
                    <th>Firma</th>
                    <th>Birim</th>
                    <th>Fiyat</th>
                    <th>Para Birimi</th>
                    <th>İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {malzemeler.length > 0 ? (
                    malzemeler.map((malzeme) => (
                      <tr key={malzeme.id}>
                        <td>
                          <strong>{malzeme.kod}</strong>
                        </td>
                        <td>{malzeme.ad}</td>
                        <td>
                          {malzeme.firma?.ad || '-'}
                        </td>
                        <td>{malzeme.birim || '-'}</td>
                        <td>
                          {malzeme.fiyat 
                            ? malzeme.fiyat.toLocaleString('tr-TR', { minimumFractionDigits: 2 })
                            : '-'}
                        </td>
                        <td>
                          {malzeme.paraBirimi ? (
                            <Badge bg="secondary">{malzeme.paraBirimi}</Badge>
                          ) : '-'}
                        </td>
                        <td>
                          <Link 
                            to={`/malzeme/edit/${malzeme.id}`}
                            className="btn btn-sm btn-warning me-2"
                          >
                            <i className="fas fa-edit"></i>
                          </Link>
                          {hasRole('Admin') && (
                            <Button
                              variant="danger"
                              size="sm"
                              onClick={() => handleDelete(malzeme.id)}
                            >
                              <i className="fas fa-trash"></i>
                            </Button>
                          )}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="7" className="text-center py-4">
                        <i className="fas fa-inbox fa-3x text-muted mb-3 d-block"></i>
                        {selectedFirmaId 
                          ? 'Bu firmaya ait malzeme bulunamadı.'
                          : 'Henüz malzeme eklenmemiş.'}
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

export default MalzemeList;
