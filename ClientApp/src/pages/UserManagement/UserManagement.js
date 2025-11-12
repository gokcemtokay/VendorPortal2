import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Badge, Form, InputGroup } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { userManagementApi } from '../../api/apiClient';
import { useFirma } from '../../context/FirmaContext';
import { toast } from 'react-toastify';
import './UserManagement.css';

const UserManagement = () => {
    const { aktiveFirma, hasYetki } = useFirma();
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        if (aktiveFirma) {
            loadUsers();
        }
    }, [aktiveFirma]);

    const loadUsers = async () => {
        try {
            setLoading(true);
            const response = await userManagementApi.getFirmaUsers(aktiveFirma.firmaId);
            setUsers(response.data);
        } catch (error) {
            toast.error('Kullanıcılar yüklenemedi');
        } finally {
            setLoading(false);
        }
    };

    const handleToggleActive = async (userId) => {
        try {
            await userManagementApi.toggleUserActive(userId);
            toast.success('Kullanıcı durumu güncellendi');
            loadUsers();
        } catch (error) {
            toast.error('İşlem başarısız');
        }
    };

    const filteredUsers = users.filter((user) =>
        user.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.email.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (!hasYetki('KullaniciYonetimi')) {
        return (
            <div className="alert alert-warning">
                <i className="fas fa-exclamation-triangle me-2"></i>
                Bu sayfaya erişim yetkiniz bulunmamaktadır.
            </div>
        );
    }

    return (
        <div className="user-management">
            <div className="page-header">
                <div>
                    <h1 className="page-title">
                        <i className="fas fa-users me-3"></i>
                        Kullanıcı Yönetimi
                    </h1>
                    <p className="page-subtitle">
                        {aktiveFirma?.firmaAd} - Kullanıcıları yönetin
                    </p>
                </div>
                <Link to="/user-management/create" className="btn btn-primary">
                    <i className="fas fa-plus me-2"></i>
                    Yeni Kullanıcı
                </Link>
            </div>

            <Card className="shadow-sm">
                <Card.Body>
                    <div className="table-controls mb-4">
                        <InputGroup className="search-box">
                            <InputGroup.Text>
                                <i className="fas fa-search"></i>
                            </InputGroup.Text>
                            <Form.Control
                                type="text"
                                placeholder="Kullanıcı ara..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </InputGroup>
                    </div>

                    {loading ? (
                        <div className="text-center py-5">
                            <div className="spinner-border text-primary" role="status">
                                <span className="visually-hidden">Yükleniyor...</span>
                            </div>
                        </div>
                    ) : (
                        <Table responsive hover className="modern-table">
                            <thead>
                                <tr>
                                    <th>Kullanıcı</th>
                                    <th>E-posta</th>
                                    <th>Telefon</th>
                                    <th>Yetkiler</th>
                                    <th>Son Giriş</th>
                                    <th>Durum</th>
                                    <th>İşlemler</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredUsers.map((user) => (
                                    <tr key={user.id}>
                                        <td>
                                            <div className="user-info">
                                                <div className="user-avatar">
                                                    {user.fullName.charAt(0).toUpperCase()}
                                                </div>
                                                <div>
                                                    <div className="user-name">{user.fullName}</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td>{user.email}</td>
                                        <td>{user.phoneNumber || '-'}</td>
                                        <td>
                                            {user.firmalar[0]?.yetkiler.slice(0, 2).map((yetki, idx) => (
                                                <Badge key={idx} bg="primary" className="me-1">
                                                    {yetki}
                                                </Badge>
                                            ))}
                                            {user.firmalar[0]?.yetkiler.length > 2 && (
                                                <Badge bg="secondary">
                                                    +{user.firmalar[0].yetkiler.length - 2}
                                                </Badge>
                                            )}
                                        </td>
                                        <td>
                                            {user.lastLoginDate
                                                ? new Date(user.lastLoginDate).toLocaleString('tr-TR')
                                                : 'Hiç giriş yapmadı'}
                                        </td>
                                        <td>
                                            <Badge bg={user.isActive ? 'success' : 'secondary'}>
                                                {user.isActive ? 'Aktif' : 'Pasif'}
                                            </Badge>
                                        </td>
                                        <td>
                                            <div className="action-buttons">
                                                <Link
                                                    to={`/user-management/edit/${user.id}`}
                                                    className="btn btn-sm btn-outline-primary"
                                                >
                                                    <i className="fas fa-edit"></i>
                                                </Link>
                                                <Button
                                                    size="sm"
                                                    variant={user.isActive ? 'outline-warning' : 'outline-success'}
                                                    onClick={() => handleToggleActive(user.id)}
                                                >
                                                    <i className={`fas fa-${user.isActive ? 'ban' : 'check'}`}></i>
                                                </Button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>
                    )}

                    {!loading && filteredUsers.length === 0 && (
                        <div className="text-center py-5 text-muted">
                            <i className="fas fa-users fa-3x mb-3"></i>
                            <p>Kullanıcı bulunamadı</p>
                        </div>
                    )}
                </Card.Body>
            </Card>
        </div>
    );
};

export default UserManagement;