import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, Table, Button, Badge, Form, InputGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { userApi } from '../../api/apiClient';

const UserManagement = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [roleFilter, setRoleFilter] = useState('');

    useEffect(() => {
        loadUsers();
    }, []);

    const loadUsers = async () => {
        try {
            setLoading(true);
            const response = await userApi.getAll();
            setUsers(response.data.data || []);
        } catch (error) {
            console.error('Kullanıcılar yüklenirken hata:', error);
            toast.error('Kullanıcılar yüklenemedi.');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, email) => {
        if (window.confirm(`${email} kullanıcısını silmek istediğinizden emin misiniz?`)) {
            try {
                await userApi.delete(id);
                toast.success('Kullanıcı başarıyla silindi.');
                loadUsers();
            } catch (error) {
                toast.error('Kullanıcı silinirken hata oluştu.');
            }
        }
    };

    const handleToggleStatus = async (id, currentStatus) => {
        try {
            await userApi.toggleStatus(id);
            toast.success(`Kullanıcı ${currentStatus ? 'pasif' : 'aktif'} yapıldı.`);
            loadUsers();
        } catch (error) {
            toast.error('Durum değiştirilemedi.');
        }
    };

    const getRoleBadge = (role) => {
        const badges = {
            'Admin': 'danger',
            'Musteri': 'primary',
            'Tedarikci': 'success'
        };
        return badges[role] || 'secondary';
    };

    const filteredUsers = users.filter(user => {
        const matchesSearch = user.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.adSoyad?.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesRole = !roleFilter || user.rol === roleFilter;
        return matchesSearch && matchesRole;
    });

    if (loading) {
        return (
            <div className="text-center py-5">
                <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Yükleniyor...</span>
                </div>
            </div>
        );
    }

    return (
        <div className="container-fluid">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>
                    <i className="fas fa-users me-2"></i>
                    Kullanıcı Yönetimi
                </h2>
                <Link to="/users/create" className="btn btn-primary">
                    <i className="fas fa-plus me-2"></i>
                    Yeni Kullanıcı
                </Link>
            </div>

            <Card>
                <Card.Body>
                    {/* Filtreler */}
                    <div className="row mb-3">
                        <div className="col-md-6">
                            <InputGroup>
                                <InputGroup.Text>
                                    <i className="fas fa-search"></i>
                                </InputGroup.Text>
                                <Form.Control
                                    type="text"
                                    placeholder="Email veya ad ile ara..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                />
                            </InputGroup>
                        </div>
                        <div className="col-md-3">
                            <Form.Select value={roleFilter} onChange={(e) => setRoleFilter(e.target.value)}>
                                <option value="">Tüm Roller</option>
                                <option value="Admin">Admin</option>
                                <option value="Musteri">Müşteri</option>
                                <option value="Tedarikci">Tedarikçi</option>
                            </Form.Select>
                        </div>
                    </div>

                    {/* Kullanıcı Tablosu */}
                    <div className="table-responsive">
                        <Table striped hover>
                            <thead>
                                <tr>
                                    <th>Ad Soyad</th>
                                    <th>Email</th>
                                    <th>Rol</th>
                                    <th>Firma</th>
                                    <th>Telefon</th>
                                    <th>Durum</th>
                                    <th>Kayıt Tarihi</th>
                                    <th style={{ width: '150px' }}>İşlemler</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredUsers.length === 0 ? (
                                    <tr>
                                        <td colSpan="8" className="text-center py-4">
                                            <i className="fas fa-inbox fa-3x text-muted mb-3 d-block"></i>
                                            <p className="text-muted">Kullanıcı bulunamadı.</p>
                                        </td>
                                    </tr>
                                ) : (
                                    filteredUsers.map(user => (
                                        <tr key={user.id}>
                                            <td>
                                                <strong>{user.adSoyad || 'Belirtilmemiş'}</strong>
                                            </td>
                                            <td>{user.email}</td>
                                            <td>
                                                <Badge bg={getRoleBadge(user.rol)}>
                                                    {user.rol}
                                                </Badge>
                                            </td>
                                            <td>{user.firmaAdi || '-'}</td>
                                            <td>{user.telefonNo || '-'}</td>
                                            <td>
                                                {user.isActive ? (
                                                    <Badge bg="success">Aktif</Badge>
                                                ) : (
                                                    <Badge bg="secondary">Pasif</Badge>
                                                )}
                                            </td>
                                            <td>
                                                {new Date(user.createdDate).toLocaleDateString('tr-TR')}
                                            </td>
                                            <td>
                                                <div className="btn-group btn-group-sm">
                                                    <Link
                                                        to={`/users/edit/${user.id}`}
                                                        className="btn btn-outline-primary"
                                                        title="Düzenle"
                                                    >
                                                        <i className="fas fa-edit"></i>
                                                    </Link>
                                                    <Button
                                                        variant={user.isActive ? "outline-warning" : "outline-success"}
                                                        onClick={() => handleToggleStatus(user.id, user.isActive)}
                                                        title={user.isActive ? "Pasif Yap" : "Aktif Yap"}
                                                    >
                                                        <i className={`fas fa-${user.isActive ? 'pause' : 'play'}`}></i>
                                                    </Button>
                                                    <Button
                                                        variant="outline-danger"
                                                        onClick={() => handleDelete(user.id, user.email)}
                                                        title="Sil"
                                                    >
                                                        <i className="fas fa-trash"></i>
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </Table>
                    </div>

                    {/* İstatistikler */}
                    <div className="row mt-4">
                        <div className="col-md-3">
                            <Card bg="primary" text="white">
                                <Card.Body>
                                    <h3>{users.length}</h3>
                                    <small>Toplam Kullanıcı</small>
                                </Card.Body>
                            </Card>
                        </div>
                        <div className="col-md-3">
                            <Card bg="danger" text="white">
                                <Card.Body>
                                    <h3>{users.filter(u => u.rol === 'Admin').length}</h3>
                                    <small>Admin</small>
                                </Card.Body>
                            </Card>
                        </div>
                        <div className="col-md-3">
                            <Card bg="info" text="white">
                                <Card.Body>
                                    <h3>{users.filter(u => u.rol === 'Musteri').length}</h3>
                                    <small>Müşteri</small>
                                </Card.Body>
                            </Card>
                        </div>
                        <div className="col-md-3">
                            <Card bg="success" text="white">
                                <Card.Body>
                                    <h3>{users.filter(u => u.rol === 'Tedarikci').length}</h3>
                                    <small>Tedarikçi</small>
                                </Card.Body>
                            </Card>
                        </div>
                    </div>
                </Card.Body>
            </Card>
        </div>
    );
};

export default UserManagement;