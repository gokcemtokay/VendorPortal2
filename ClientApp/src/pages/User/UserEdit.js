import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Card, Form, Button, Row, Col, Alert, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { userApi, firmaApi } from '../../api/apiClient';

const UserEdit = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [firmalar, setFirmalar] = useState([]);
    const [formData, setFormData] = useState({
        id: '',
        email: '',
        adSoyad: '',
        telefonNo: '',
        rol: '',
        firmaId: '',
        isActive: true,
        changePassword: false,
        newPassword: '',
        confirmPassword: ''
    });
    const [errors, setErrors] = useState({});

    useEffect(() => {
        loadUser();
        loadFirmalar();
    }, [id]);

    const loadUser = async () => {
        try {
            setLoading(true);
            const response = await userApi.getById(id);
            const user = response.data.data;
            setFormData({
                id: user.id,
                email: user.email,
                adSoyad: user.adSoyad || '',
                telefonNo: user.telefonNo || '',
                rol: user.rol,
                firmaId: user.firmaId || '',
                isActive: user.isActive,
                changePassword: false,
                newPassword: '',
                confirmPassword: ''
            });
        } catch (error) {
            console.error('Kullanıcı yüklenirken hata:', error);
            toast.error('Kullanıcı bulunamadı.');
            navigate('/users');
        } finally {
            setLoading(false);
        }
    };

    const loadFirmalar = async () => {
        try {
            const response = await firmaApi.getAll();
            setFirmalar(response.data.data || []);
        } catch (error) {
            console.error('Firmalar yüklenirken hata:', error);
        }
    };

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
        // Clear error when user types
        if (errors[name]) {
            setErrors(prev => ({ ...prev, [name]: '' }));
        }
    };

    const validateForm = () => {
        const newErrors = {};

        if (!formData.email) {
            newErrors.email = 'Email zorunludur.';
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = 'Geçerli bir email giriniz.';
        }

        if (!formData.adSoyad) {
            newErrors.adSoyad = 'Ad Soyad zorunludur.';
        }

        if (!formData.rol) {
            newErrors.rol = 'Rol seçimi zorunludur.';
        }

        if ((formData.rol === 'Musteri' || formData.rol === 'Tedarikci') && !formData.firmaId) {
            newErrors.firmaId = 'Müşteri ve Tedarikçi için firma seçimi zorunludur.';
        }

        if (formData.changePassword) {
            if (!formData.newPassword) {
                newErrors.newPassword = 'Yeni şifre zorunludur.';
            } else if (formData.newPassword.length < 6) {
                newErrors.newPassword = 'Şifre en az 6 karakter olmalıdır.';
            }

            if (formData.newPassword !== formData.confirmPassword) {
                newErrors.confirmPassword = 'Şifreler eşleşmiyor.';
            }
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateForm()) {
            toast.error('Lütfen formu eksiksiz doldurun.');
            return;
        }

        try {
            setSaving(true);
            const submitData = {
                id: formData.id,
                email: formData.email,
                adSoyad: formData.adSoyad,
                telefonNo: formData.telefonNo,
                rol: formData.rol,
                firmaId: formData.firmaId || null,
                isActive: formData.isActive
            };

            if (formData.changePassword) {
                submitData.password = formData.newPassword;
            }

            await userApi.update(submitData);
            toast.success('Kullanıcı başarıyla güncellendi!');
            navigate('/users');
        } catch (error) {
            console.error('Kullanıcı güncellenirken hata:', error);
            toast.error(error.response?.data?.message || 'Kullanıcı güncellenemedi.');
        } finally {
            setSaving(false);
        }
    };

    const filteredFirmalar = firmalar.filter(firma => {
        if (formData.rol === 'Musteri') {
            return firma.firmaTipi === 1 || firma.firmaTipi === 3;
        } else if (formData.rol === 'Tedarikci') {
            return firma.firmaTipi === 2 || firma.firmaTipi === 3;
        }
        return false;
    });

    if (loading) {
        return (
            <div className="text-center py-5">
                <Spinner animation="border" variant="primary" />
                <p className="mt-3">Kullanıcı yükleniyor...</p>
            </div>
        );
    }

    return (
        <div className="container-fluid">
            <div className="mb-4">
                <h2>
                    <i className="fas fa-user-edit me-2"></i>
                    Kullanıcı Düzenle
                </h2>
            </div>

            <Row>
                <Col md={8}>
                    <Card>
                        <Card.Body>
                            <Form onSubmit={handleSubmit}>
                                {/* Kişisel Bilgiler */}
                                <h5 className="mb-3">
                                    <i className="fas fa-user me-2"></i>
                                    Kişisel Bilgiler
                                </h5>

                                <Row>
                                    <Col md={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Ad Soyad <span className="text-danger">*</span></Form.Label>
                                            <Form.Control
                                                type="text"
                                                name="adSoyad"
                                                value={formData.adSoyad}
                                                onChange={handleChange}
                                                isInvalid={!!errors.adSoyad}
                                            />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.adSoyad}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Col>

                                    <Col md={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Telefon</Form.Label>
                                            <Form.Control
                                                type="tel"
                                                name="telefonNo"
                                                value={formData.telefonNo}
                                                onChange={handleChange}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>

                                {/* Hesap Bilgileri */}
                                <h5 className="mb-3 mt-4">
                                    <i className="fas fa-envelope me-2"></i>
                                    Hesap Bilgileri
                                </h5>

                                <Form.Group className="mb-3">
                                    <Form.Label>Email <span className="text-danger">*</span></Form.Label>
                                    <Form.Control
                                        type="email"
                                        name="email"
                                        value={formData.email}
                                        onChange={handleChange}
                                        isInvalid={!!errors.email}
                                    />
                                    <Form.Control.Feedback type="invalid">
                                        {errors.email}
                                    </Form.Control.Feedback>
                                </Form.Group>

                                {/* Şifre Değiştirme */}
                                <Form.Group className="mb-3">
                                    <Form.Check
                                        type="checkbox"
                                        name="changePassword"
                                        checked={formData.changePassword}
                                        onChange={handleChange}
                                        label="Şifre değiştir"
                                    />
                                </Form.Group>

                                {formData.changePassword && (
                                    <Row>
                                        <Col md={6}>
                                            <Form.Group className="mb-3">
                                                <Form.Label>Yeni Şifre <span className="text-danger">*</span></Form.Label>
                                                <Form.Control
                                                    type="password"
                                                    name="newPassword"
                                                    value={formData.newPassword}
                                                    onChange={handleChange}
                                                    isInvalid={!!errors.newPassword}
                                                    placeholder="En az 6 karakter"
                                                />
                                                <Form.Control.Feedback type="invalid">
                                                    {errors.newPassword}
                                                </Form.Control.Feedback>
                                            </Form.Group>
                                        </Col>

                                        <Col md={6}>
                                            <Form.Group className="mb-3">
                                                <Form.Label>Şifre Tekrar <span className="text-danger">*</span></Form.Label>
                                                <Form.Control
                                                    type="password"
                                                    name="confirmPassword"
                                                    value={formData.confirmPassword}
                                                    onChange={handleChange}
                                                    isInvalid={!!errors.confirmPassword}
                                                    placeholder="Şifreyi tekrar girin"
                                                />
                                                <Form.Control.Feedback type="invalid">
                                                    {errors.confirmPassword}
                                                </Form.Control.Feedback>
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                )}

                                {/* Rol ve Firma */}
                                <h5 className="mb-3 mt-4">
                                    <i className="fas fa-building me-2"></i>
                                    Rol ve Firma
                                </h5>

                                <Row>
                                    <Col md={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Rol <span className="text-danger">*</span></Form.Label>
                                            <Form.Select
                                                name="rol"
                                                value={formData.rol}
                                                onChange={handleChange}
                                                isInvalid={!!errors.rol}
                                            >
                                                <option value="">Rol Seçin</option>
                                                <option value="Admin">Admin</option>
                                                <option value="Musteri">Müşteri</option>
                                                <option value="Tedarikci">Tedarikçi</option>
                                            </Form.Select>
                                            <Form.Control.Feedback type="invalid">
                                                {errors.rol}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Col>

                                    <Col md={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>
                                                Firma
                                                {(formData.rol === 'Musteri' || formData.rol === 'Tedarikci') && (
                                                    <span className="text-danger"> *</span>
                                                )}
                                            </Form.Label>
                                            <Form.Select
                                                name="firmaId"
                                                value={formData.firmaId}
                                                onChange={handleChange}
                                                isInvalid={!!errors.firmaId}
                                                disabled={formData.rol === 'Admin'}
                                            >
                                                <option value="">
                                                    {formData.rol === 'Admin' ? 'Firma Gerekmez' : 'Firma Seçin'}
                                                </option>
                                                {filteredFirmalar.map(firma => (
                                                    <option key={firma.id} value={firma.id}>
                                                        {firma.ad}
                                                    </option>
                                                ))}
                                            </Form.Select>
                                            <Form.Control.Feedback type="invalid">
                                                {errors.firmaId}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Col>
                                </Row>

                                {/* Durum */}
                                <Form.Group className="mb-3">
                                    <Form.Check
                                        type="checkbox"
                                        name="isActive"
                                        checked={formData.isActive}
                                        onChange={handleChange}
                                        label="Aktif kullanıcı"
                                    />
                                </Form.Group>

                                {/* Buttons */}
                                <div className="d-flex justify-content-end gap-2 mt-4">
                                    <Button
                                        variant="secondary"
                                        onClick={() => navigate('/users')}
                                        disabled={saving}
                                    >
                                        <i className="fas fa-times me-2"></i>
                                        İptal
                                    </Button>
                                    <Button
                                        variant="primary"
                                        type="submit"
                                        disabled={saving}
                                    >
                                        {saving ? (
                                            <>
                                                <span className="spinner-border spinner-border-sm me-2" />
                                                Kaydediliyor...
                                            </>
                                        ) : (
                                            <>
                                                <i className="fas fa-save me-2"></i>
                                                Değişiklikleri Kaydet
                                            </>
                                        )}
                                    </Button>
                                </div>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4}>
                    <Card bg="warning" text="dark">
                        <Card.Body>
                            <h5><i className="fas fa-exclamation-triangle me-2"></i>Dikkat</h5>
                            <ul className="mb-0">
                                <li>Kullanıcı rolünü değiştirirken dikkatli olun</li>
                                <li>Şifre değiştirmek isteğe bağlıdır</li>
                                <li>Pasif kullanıcılar sisteme giriş yapamaz</li>
                            </ul>
                        </Card.Body>
                    </Card>

                    <Card className="mt-3 bg-info text-white">
                        <Card.Body>
                            <h6><i className="fas fa-info-circle me-2"></i>Kullanıcı Bilgileri</h6>
                            <p className="mb-1"><strong>ID:</strong> {formData.id}</p>
                            <p className="mb-1"><strong>Email:</strong> {formData.email}</p>
                            <p className="mb-0"><strong>Durum:</strong> {formData.isActive ? 'Aktif' : 'Pasif'}</p>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default UserEdit;