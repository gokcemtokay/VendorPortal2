import React, { useState, useEffect } from 'react';
import { Card, Row, Col, Badge, ProgressBar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import {
    BarChart, Bar, PieChart, Pie, LineChart, Line,
    XAxis, YAxis, CartesianGrid, Tooltip, Legend,
    ResponsiveContainer, Cell
} from 'recharts';
import './Dashboard.css';
import apiClient from '../../api/apiClient';

const Dashboard = () => {
    const { user } = useAuth();
    const [stats, setStats] = useState({
        totalTeklif: 14,
        acikOran: 78,
        yeniAlimTarzi: '+%15',
        bekleyenTeklif: 2,
        onayBekleyen: 12,
        aktifIhale: 529,
        tedarikciSayisi: 3
    });

    const [loading, setLoading] = useState(true);

    // Grafik verileri
    const siparisGrafik = [
        { ay: 'Oca', bekleyen: 23, onaylanan: 45 },
        { ay: 'Şub', bekleyen: 15, onaylanan: 62 },
        { ay: 'Mar', bekleyen: 28, onaylanan: 51 },
        { ay: 'Nis', bekleyen: 19, onaylanan: 48 },
        { ay: 'May', bekleyen: 32, onaylanan: 67 },
        { ay: 'Haz', bekleyen: 25, onaylanan: 58 }
    ];

    const ihaleDagilim = [
        { name: 'Açık İhale', value: 134, color: '#2563EB' },
        { name: 'Kapalı İhale', value: 63, color: '#10B981' },
        { name: 'Davetli İhale', value: 28, color: '#F59E0B' }
    ];

    const aylikTrend = [
        { gun: '1', deger: 4200 },
        { gun: '5', deger: 3800 },
        { gun: '10', deger: 4500 },
        { gun: '15', deger: 5200 },
        { gun: '20', deger: 4800 },
        { gun: '25', deger: 5500 },
        { gun: '30', deger: 6100 }
    ];

    const sonAktiviteler = [
        {
            id: 1,
            tip: 'siparis',
            baslik: 'Sipariş teslim edildi.',
            detay: 'Merkez Plastik A.Ş',
            tarih: '20 Temmuz 2022',
            durum: 'success'
        },
        {
            id: 2,
            tip: 'siparis',
            baslik: 'Sipariş alındı.',
            detay: 'Demir Ambalaj A.Ş',
            tarih: '20 Temmuz 2022',
            durum: 'info'
        },
        {
            id: 3,
            tip: 'teslimat',
            baslik: 'Teslimat süreci başladı.',
            detay: 'Maya Kimya Sanayii',
            tarih: '19 Temmuz 2022',
            durum: 'warning'
        },
        {
            id: 4,
            tip: 'siparis',
            baslik: 'Sipariş onaylandı.',
            detay: 'Karaman Çelik Sanayii',
            tarih: '19 Temmuz 2022',
            durum: 'primary'
        },
        {
            id: 5,
            tip: 'teklif',
            baslik: '2 teklif',
            detay: 'Yeni teklif',
            tarih: '',
            durum: 'light'
        }
    ];

    useEffect(() => {
        loadDashboardData();
    }, []);

    const loadDashboardData = async () => {
        try {
            // API'den gerçek istatistikleri çek
            const response = await apiClient.get('/api/Dashboard/GetStats');

            setStats({
                totalTeklif: response.data.totalTeklif,
                acikOran: response.data.acikOran,
                yeniAlimTarzi: response.data.yeniAlimTarzi,
                bekleyenTeklif: response.data.bekleyenTeklif,
                onayBekleyen: response.data.onayBekleyen,
                aktifIhale: response.data.aktifIhale,
                tedarikciSayisi: response.data.tedarikciSayisi
            });
        } catch (error) {
            console.error('Dashboard verileri yüklenirken hata:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="dashboard-loading">
                <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Yükleniyor...</span>
                </div>
            </div>
        );
    }

    return (
        <div className="modern-dashboard">
            {/* Header */}
            <div className="dashboard-header">
                <div>
                    <h1 className="dashboard-title">Hoşgeldiniz Gökçem Tokay!</h1>
                    <p className="dashboard-subtitle">Size ve tedarikçilerinizle dair istatistikleri görebilirsiniz.</p>
                </div>
                <div className="header-actions">
                    <input
                        type="text"
                        className="search-input"
                        placeholder="Search"
                    />
                </div>
            </div>

            {/* Ana İstatistikler */}
            <Row className="g-4 mb-4">
                <Col lg={3} md={6}>
                    <Card className="stat-card stat-card-primary">
                        <Card.Body>
                            <div className="stat-icon">
                                <i className="fas fa-file-alt"></i>
                            </div>
                            <div className="stat-content">
                                <div className="stat-value">{stats.totalTeklif}</div>
                                <div className="stat-label">Teklif</div>
                                <div className="stat-sublabel">Ana Sayfa</div>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3} md={6}>
                    <Card className="stat-card stat-card-info">
                        <Card.Body>
                            <div className="stat-icon">
                                <i className="fas fa-folder-open"></i>
                            </div>
                            <div className="stat-content">
                                <div className="stat-value">%{stats.acikOran}</div>
                                <div className="stat-label">Açık</div>
                                <div className="stat-sublabel">Teklif durum</div>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3} md={6}>
                    <Card className="stat-card stat-card-success">
                        <Card.Body>
                            <div className="stat-icon">
                                <i className="fas fa-shopping-cart"></i>
                            </div>
                            <div className="stat-content">
                                <div className="stat-value">{stats.yeniAlimTarzi}</div>
                                <div className="stat-label">Yeni Alım Tarzı</div>
                                <div className="stat-sublabel">Alım fırsatı</div>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3} md={6}>
                    <Card className="stat-card stat-card-light">
                        <Card.Body>
                            <div className="stat-content-full">
                                <div className="stat-value">{stats.bekleyenTeklif}</div>
                                <div className="stat-label">Teklif</div>
                                <div className="stat-sublabel">Teklif bekliyor 2 teklif için taktim tutunuz.</div>
                                <button className="btn btn-primary btn-sm mt-2">
                                    Detayları Görüntüle
                                </button>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Orta Bölüm - Grafikler */}
            <Row className="g-4 mb-4">
                {/* Sipariş Grafiği */}
                <Col lg={4}>
                    <Card className="chart-card">
                        <Card.Body>
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <h6 className="chart-title">Son 1 Hafta</h6>
                                <i className="fas fa-ellipsis-h text-muted"></i>
                            </div>
                            <div className="chart-main-stat">
                                <div className="chart-value">23</div>
                                <div className="chart-label">Onay bekliyor.</div>
                            </div>
                            <ResponsiveContainer width="100%" height={200}>
                                <BarChart data={siparisGrafik.slice(-4)}>
                                    <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                                    <XAxis dataKey="ay" tick={{ fontSize: 12 }} />
                                    <YAxis tick={{ fontSize: 12 }} />
                                    <Tooltip />
                                    <Bar dataKey="bekleyen" fill="#06B6D4" radius={[8, 8, 0, 0]} />
                                    <Bar dataKey="onaylanan" fill="#2563EB" radius={[8, 8, 0, 0]} />
                                </BarChart>
                            </ResponsiveContainer>
                        </Card.Body>
                    </Card>
                </Col>

                {/* İhale Dağılımı */}
                <Col lg={4}>
                    <Card className="chart-card">
                        <Card.Body>
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <h6 className="chart-title">Son 1 Hafta</h6>
                                <i className="fas fa-ellipsis-h text-muted"></i>
                            </div>
                            <div className="chart-main-stat">
                                <div className="chart-value">134</div>
                                <div className="chart-label">Tedbiksel süre içinde.</div>
                            </div>
                            <ResponsiveContainer width="100%" height={200}>
                                <PieChart>
                                    <Pie
                                        data={ihaleDagilim}
                                        cx="50%"
                                        cy="50%"
                                        innerRadius={60}
                                        outerRadius={80}
                                        paddingAngle={5}
                                        dataKey="value"
                                    >
                                        {ihaleDagilim.map((entry, index) => (
                                            <Cell key={`cell-${index}`} fill={entry.color} />
                                        ))}
                                    </Pie>
                                    <Tooltip />
                                </PieChart>
                            </ResponsiveContainer>
                            <div className="text-center mt-2">
                                <span className="pie-center-value">63</span>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>

                {/* Aylık Trend */}
                <Col lg={4}>
                    <Card className="chart-card chart-card-gradient">
                        <Card.Body>
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <div>
                                    <i className="fas fa-plus-circle me-2"></i>
                                    <i className="fas fa-ellipsis-h text-white"></i>
                                </div>
                            </div>
                            <div className="chart-main-stat text-white">
                                <div className="chart-value">529</div>
                                <div className="chart-label">Şikayet tamlandı.</div>
                            </div>
                            <ResponsiveContainer width="100%" height={200}>
                                <LineChart data={aylikTrend}>
                                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                                    <XAxis dataKey="gun" tick={{ fill: 'white', fontSize: 12 }} />
                                    <YAxis tick={{ fill: 'white', fontSize: 12 }} />
                                    <Tooltip />
                                    <Line
                                        type="monotone"
                                        dataKey="deger"
                                        stroke="#10B981"
                                        strokeWidth={3}
                                        dot={{ fill: '#10B981', r: 4 }}
                                    />
                                </LineChart>
                            </ResponsiveContainer>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Alt Bölüm */}
            <Row className="g-4">
                {/* Tedarikçi Sayısı */}
                <Col lg={4}>
                    <Card className="chart-card">
                        <Card.Body>
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <div>
                                    <i className="fas fa-users me-2 text-primary"></i>
                                    <i className="fas fa-ellipsis-h text-muted"></i>
                                </div>
                            </div>
                            <div className="chart-main-stat">
                                <div className="chart-value">{stats.tedarikciSayisi}</div>
                                <div className="chart-label">tedarikçi</div>
                                <div className="chart-sublabel">Verdiğiniz ziyaret için tedarikçi.</div>
                            </div>
                            <ResponsiveContainer width="100%" height={120}>
                                <LineChart data={[
                                    { x: 1, y: 2 },
                                    { x: 2, y: 3 },
                                    { x: 3, y: 2.5 },
                                    { x: 4, y: 3.5 },
                                    { x: 5, y: 3 }
                                ]}>
                                    <Line
                                        type="monotone"
                                        dataKey="y"
                                        stroke="#06B6D4"
                                        strokeWidth={3}
                                        dot={false}
                                    />
                                </LineChart>
                            </ResponsiveContainer>
                        </Card.Body>
                    </Card>
                </Col>

                {/* Duyurular */}
                <Col lg={4}>
                    <Card className="announcements-card">
                        <Card.Header className="announcements-header">
                            <div className="d-flex justify-content-between align-items-center">
                                <div>
                                    <i className="fas fa-bell me-2"></i>
                                    <span>Duyurular</span>
                                </div>
                                <i className="fas fa-ellipsis-h"></i>
                            </div>
                        </Card.Header>
                        <Card.Body className="p-0">
                            <div className="announcement-item">
                                <div className="announcement-icon bg-primary">
                                    <i className="fas fa-info"></i>
                                </div>
                                <div className="announcement-content">
                                    <div className="announcement-date">20 Temmuz 2022</div>
                                    <div className="announcement-title">14 numune</div>
                                    <div className="announcement-text">Revizyon istendi</div>
                                </div>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>

                {/* Son Aktiviteler */}
                <Col lg={4}>
                    <Card className="activity-card">
                        <Card.Body>
                            <div className="activity-header">
                                <i className="fas fa-calendar-alt me-2"></i>
                                <span>Son Aktiviteler</span>
                            </div>

                            <div className="activity-timeline">
                                {sonAktiviteler.map((aktivite, index) => (
                                    <div key={aktivite.id} className="activity-item">
                                        <div className="activity-date">{aktivite.tarih}</div>
                                        <div className="activity-content">
                                            <div className={`activity-dot bg-${aktivite.durum}`}></div>
                                            <div className="activity-details">
                                                <div className="activity-title">{aktivite.baslik}</div>
                                                <div className="activity-subtitle">{aktivite.detay}</div>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Hızlı İşlemler */}
            <Row className="g-4 mt-3">
                <Col>
                    <Card className="quick-actions-card">
                        <Card.Body>
                            <h6 className="mb-4">
                                <i className="fas fa-bolt me-2"></i>
                                Hızlı İşlemler
                            </h6>
                            <Row className="g-3">
                                <Col md={3}>
                                    <Link to="/firma/create" className="quick-action-btn">
                                        <i className="fas fa-building"></i>
                                        <span>Yeni Firma</span>
                                    </Link>
                                </Col>
                                <Col md={3}>
                                    <Link to="/ihale/create" className="quick-action-btn">
                                        <i className="fas fa-gavel"></i>
                                        <span>İhale Aç</span>
                                    </Link>
                                </Col>
                                <Col md={3}>
                                    <Link to="/siparis/create" className="quick-action-btn">
                                        <i className="fas fa-shopping-cart"></i>
                                        <span>Sipariş Oluştur</span>
                                    </Link>
                                </Col>
                                <Col md={3}>
                                    <Link to="/malzeme" className="quick-action-btn">
                                        <i className="fas fa-box"></i>
                                        <span>Malzemeler</span>
                                    </Link>
                                </Col>
                            </Row>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default Dashboard;