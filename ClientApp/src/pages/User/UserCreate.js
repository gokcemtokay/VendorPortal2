import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { userApi, firmaApi } from '../../api/apiClient';

const UserCreate = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [firmalar, setFirmalar] = useState([]);
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        confirmPassword: '',
        adSoyad: '',
        telefonNo: '',
        rol: 'Musteri',
        firmaId: '',
        isActive: true
    });
    const [errors, setErrors] = useState({});

    useEffect(() => {
        loadFirmalar();
    }, []);

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

        if (!formData.password) {
            newErrors.password = 'Şifre zorunludur.';
        } else if (formData.password.length < 6) {
            newErrors.password = 'Şifre en az 6 karakter olmalıdır.';
        }

        if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = 'Şifreler eşleşmiyor.';
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
            setLoading(true);
            const submitData = {
                ...formData,
                firmaId: formData.firmaId || null
            };
            delete submitData.confirmPassword;

            await userApi.create(submitData);
            toast.success('Kullanıcı başarıyla oluşturuldu!');
            navigate('/users');
        } catch (error) {
            console.error('Kullanıcı oluşturulurken hata:', error);
            toast.error(error.response?.data?.message || 'Kullanıcı oluşturulamadı.');
        } finally {
            setLoading(false);
        }
    };

    const filteredFirmalar = firmalar.filter(firma => {
        if (formData.rol === 'Musteri') {
            return firma.firmaTipi === 1 || firma.firmaTipi === 3; // Müşteri veya Her İkisi
        } else if (formData.rol === 'Tedarikci') {
            return firma.firmaTipi === 2 || firma.firmaTipi === 3; // Tedarikçi veya Her İkisi
        }
        return false;
    });

    return (
        <div className="container-fluid">
            <div className="mb-4">
                <h2>
                    <i className="fas fa-user-plus me-2"></i>
                    Yeni Kullanıcı Oluştur
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
                                                placeholder="Ahmet Yılmaz"
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
                                                placeholder="0555 123 4567"
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>

                                {/* Hesap Bilgileri */}
                                <h5 className="mb-3 mt-4">
                                    <i className="fas fa-key me-2"></i>
                                    Hesap Bilgileri
                                </h5>

                                <Row>
                                    <Col md={12}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Email <span className="text-danger">*</span></Form.Label>
                                            <Form.Control
                                                type="email"
                                                name="email"
                                                value={formData.email}
                                                onChange={handleChange}
                                                isInvalid={!!errors.email}
                                                placeholder="kullanici@ornek.com"
                                            />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.email}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Col>

                                    <Col md={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Şifre <span className="text-danger">*</span></Form.Label>
                                            <Form.Control
                                                type="password"
                                                name="password"
                                                value={formData.password}
                                                onChange={handleChange}
                                                isInvalid={!!errors.password}
                                                placeholder="En az 6 karakter"
                                            />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.password}
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
                                            {formData.rol !== 'Admin' && filteredFirmalar.length === 0 && (
                                                <Form.Text className="text-warning">
                                                    Uygun firma bulunamadı. Önce firma oluşturun.
                                                </Form.Text>
                                            )}
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
                                        disabled={loading}
                                    >
                                        <i className="fas fa-times me-2"></i>
                                        İptal
                                    </Button>
                                    <Button
                                        variant="primary"
                                        type="submit"
                                        disabled={loading}
                                    >
                                        {loading ? (
                                            <>
                                                <span className="spinner-border spinner-border-sm me-2" />
                                                Oluşturuluyor...
                                            </>
                                        ) : (
                                            <>
                                                <i className="fas fa-save me-2"></i>
                                                Kullanıcı Oluştur
                                            </>
                                        )}
                                    </Button>
                                </div>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4}>
                    <Card bg="info" text="white">
                        <Card.Body>
                            <h5><i className="fas fa-info-circle me-2"></i>Bilgilendirme</h5>
                            <ul className="mb-0">
                                <li>Admin kullanıcıları firma'ya bağlı olmaz</li>
                                <li>Müşteri ve Tedarikçi kullanıcıları mutlaka bir firmaya bağlı olmalıdır</li>
                                <li>Şifre en az 6 karakter olmalıdır</li>
                                <li>Email adresi benzersiz olmalıdır</li>
                            </ul>
                        </Card.Body>
                    </Card>

                    <Card className="mt-3">
                        <Card.Body>
                            <h6><i className="fas fa-shield-alt me-2"></i>Rol Yetkileri</h6>
                            <Alert variant="danger" className="mb-2 py-2">
                                <strong>Admin:</strong> Tüm yetkilere sahip
                            </Alert>
                            <Alert variant="primary" className="mb-2 py-2">
                                <strong>Müşteri:</strong> Sipariş ve ihale oluşturabilir
                            </Alert>
                            <Alert variant="success" className="mb-0 py-2">
                                <strong>Tedarikçi:</strong> Teklif verebilir
                            </Alert>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default UserCreate;