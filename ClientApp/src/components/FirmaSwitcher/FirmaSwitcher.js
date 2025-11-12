import React from 'react';
import { NavDropdown } from 'react-bootstrap';
import { useFirma } from '../../context/FirmaContext';
import './FirmaSwitcher.css';

const FirmaSwitcher = () => {
    const { aktiveFirma, userFirmalar, changeFirma, loading } = useFirma();

    if (loading || userFirmalar.length === 0) return null;

    // Tek firma varsa dropdown gösterme
    if (userFirmalar.length === 1) {
        return (
            <div className="single-firma-display">
                <i className="fas fa-building me-2"></i>
                <span>{aktiveFirma?.firmaAd}</span>
            </div>
        );
    }

    return (
        <NavDropdown
            title={
                <span className="firma-switcher-trigger">
                    <i className="fas fa-building me-2"></i>
                    {aktiveFirma?.firmaAd || 'Firma Seçin'}
                    <i className="fas fa-chevron-down ms-2 small"></i>
                </span>
            }
            id="firma-switcher"
            align="end"
            className="firma-switcher-dropdown"
        >
            <div className="firma-switcher-header">
                <h6>Firma Değiştir</h6>
                <p className="text-muted small mb-0">
                    {userFirmalar.length} firma yetkisi
                </p>
            </div>

            {userFirmalar.map((firma) => (
                <NavDropdown.Item
                    key={firma.firmaId}
                    onClick={() => changeFirma(firma.firmaId)}
                    className={`firma-item ${aktiveFirma?.firmaId === firma.firmaId ? 'active' : ''}`}
                >
                    <div className="firma-item-content">
                        <div className="firma-name">
                            {firma.firmaAd}
                            {aktiveFirma?.firmaId === firma.firmaId && (
                                <i className="fas fa-check ms-2 text-primary"></i>
                            )}
                        </div>
                        <div className="firma-yetkiler">
                            {firma.yetkiler.slice(0, 2).map((yetki, idx) => (
                                <span key={idx} className="badge bg-secondary me-1">
                                    {yetki}
                                </span>
                            ))}
                            {firma.yetkiler.length > 2 && (
                                <span className="badge bg-secondary">
                                    +{firma.yetkiler.length - 2}
                                </span>
                            )}
                        </div>
                    </div>
                </NavDropdown.Item>
            ))}
        </NavDropdown>
    );
};

export default FirmaSwitcher;