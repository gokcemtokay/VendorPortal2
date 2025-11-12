import React, { useState, useEffect } from 'react';
import { NavDropdown, Badge } from 'react-bootstrap';
import { notificationApi } from '../../api/apiClient';
import { toast } from 'react-toastify';
import { Link } from 'react-router-dom';
import './NotificationDropdown.css';

const NotificationDropdown = () => {
    const [notifications, setNotifications] = useState([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        loadNotifications();
        loadUnreadCount();

        // Her 30 saniyede bir yenile
        const interval = setInterval(() => {
            loadUnreadCount();
        }, 30000);

        return () => clearInterval(interval);
    }, []);

    const loadNotifications = async () => {
        try {
            setLoading(true);
            const response = await notificationApi.getMyNotifications(false);
            setNotifications(response.data);
        } catch (error) {
            console.error('Bildirimler yüklenemedi:', error);
        } finally {
            setLoading(false);
        }
    };

    const loadUnreadCount = async () => {
        try {
            const response = await notificationApi.getUnreadCount();
            setUnreadCount(response.data.count);
        } catch (error) {
            console.error('Okunmamış bildirim sayısı yüklenemedi:', error);
        }
    };

    const handleMarkAsRead = async (id) => {
        try {
            await notificationApi.markAsRead(id);
            loadNotifications();
            loadUnreadCount();
        } catch (error) {
            toast.error('Bildirim okundu olarak işaretlenemedi');
        }
    };

    const handleMarkAllAsRead = async () => {
        try {
            await notificationApi.markAllAsRead();
            loadNotifications();
            loadUnreadCount();
            toast.success('Tüm bildirimler okundu olarak işaretlendi');
        } catch (error) {
            toast.error('İşlem başarısız');
        }
    };

    const getIcon = (tip) => {
        const icons = {
            1: 'info-circle',
            2: 'shopping-cart',
            3: 'clock',
            4: 'check-circle',
            5: 'times-circle',
            6: 'gavel',
            7: 'envelope',
            8: 'file-alt',
            9: 'thumbs-up',
            10: 'thumbs-down',
            11: 'building',
            12: 'truck'
        };
        return icons[tip] || 'bell';
    };

    const getColorClass = (tip) => {
        const colors = {
            1: 'info',
            2: 'primary',
            3: 'warning',
            4: 'success',
            5: 'danger',
            6: 'info',
            7: 'secondary',
            8: 'primary',
            9: 'success',
            10: 'danger',
            11: 'warning',
            12: 'warning'
        };
        return colors[tip] || 'secondary';
    };

    return (
        <NavDropdown
            title={
                <span className="notification-trigger">
                    <i className="fas fa-bell"></i>
                    {unreadCount > 0 && (
                        <Badge bg="danger" pill className="notification-badge">
                            {unreadCount > 9 ? '9+' : unreadCount}
                        </Badge>
                    )}
                </span>
            }
            id="notification-dropdown"
            align="end"
            className="notification-dropdown"
            onToggle={(isOpen) => {
                if (isOpen) {
                    loadNotifications();
                }
            }}
        >
            <div className="notification-header">
                <h6>Bildirimler</h6>
                {unreadCount > 0 && (
                    <button
                        className="btn btn-link btn-sm text-primary"
                        onClick={handleMarkAllAsRead}
                    >
                        Tümünü Okundu İşaretle
                    </button>
                )}
            </div>

            <div className="notification-list">
                {loading ? (
                    <div className="text-center py-3">
                        <div className="spinner-border spinner-border-sm" role="status">
                            <span className="visually-hidden">Yükleniyor...</span>
                        </div>
                    </div>
                ) : notifications.length === 0 ? (
                    <div className="notification-item empty">
                        <i className="fas fa-inbox mb-2"></i>
                        <p>Bildirim bulunmuyor</p>
                    </div>
                ) : (
                    notifications.slice(0, 10).map((notif) => (
                        <div
                            key={notif.id}
                            className={`notification-item ${!notif.okundu ? 'unread' : ''}`}
                            onClick={() => handleMarkAsRead(notif.id)}
                        >
                            <div className={`notification-icon bg-${getColorClass(notif.tip)}`}>
                                <i className={`fas fa-${getIcon(notif.tip)}`}></i>
                            </div>
                            <div className="notification-content">
                                <div className="notification-title">{notif.baslik}</div>
                                {notif.icerik && (
                                    <div className="notification-body">{notif.icerik}</div>
                                )}
                                <div className="notification-time">
                                    {new Date(notif.createdDate).toLocaleString('tr-TR')}
                                </div>
                            </div>
                            {!notif.okundu && <div className="unread-dot"></div>}
                        </div>
                    ))
                )}
            </div>

            {notifications.length > 10 && (
                <NavDropdown.Item as={Link} to="/notifications" className="text-center">
                    <strong>Tüm Bildirimleri Gör</strong>
                </NavDropdown.Item>
            )}
        </NavDropdown>
    );
};

export default NotificationDropdown;