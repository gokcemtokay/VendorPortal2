import React from 'react';
import { Dropdown } from 'react-bootstrap';
import { useFirma } from '../../context/FirmaContext';

const FirmaSwitcher = () => {
    const { currentFirma, firmalar = [], selectFirma, loading } = useFirma();

    // Firmalar yüklenmemişse veya yoksa gösterme
    if (loading || !firmalar || firmalar.length === 0) {
        return null;
    }

    // Sadece bir firma varsa gösterme
    if (firmalar.length === 1) {
        return (
            <div className="text-white me-3">
                <small>
                    <i className="fas fa-building me-1"></i>
                    {currentFirma?.ad || 'Firma Yükleniyor...'}
                </small>
            </div>
        );
    }

    // Birden fazla firma varsa dropdown göster
    return (
        <Dropdown className="me-3">
            <Dropdown.Toggle variant="outline-light" size="sm" id="firma-dropdown">
                <i className="fas fa-building me-1"></i>
                {currentFirma?.ad || 'Firma Seç'}
            </Dropdown.Toggle>

            <Dropdown.Menu>
                <Dropdown.Header>Firmalar</Dropdown.Header>
                {firmalar.map((firma) => (
                    <Dropdown.Item
                        key={firma.id}
                        active={currentFirma?.id === firma.id}
                        onClick={() => selectFirma(firma.id)}
                    >
                        {firma.ad}
                        {currentFirma?.id === firma.id && (
                            <i className="fas fa-check ms-2 text-success"></i>
                        )}
                    </Dropdown.Item>
                ))}
            </Dropdown.Menu>
        </Dropdown>
    );
};

export default FirmaSwitcher;