import React, { createContext, useState, useContext, useEffect } from 'react';
import { userManagementApi } from '../api/apiClient';
import { useAuth } from './AuthContext';

const FirmaContext = createContext();

export const FirmaProvider = ({ children }) => {
    const { user, isAuthenticated } = useAuth();
    const [aktiveFirma, setAktiveFirma] = useState(null);
    const [userFirmalar, setUserFirmalar] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (isAuthenticated && user) {
            loadUserFirmalar();
        }
    }, [isAuthenticated, user]);

    const loadUserFirmalar = async () => {
        try {
            const response = await userManagementApi.getCurrentUserInfo();
            const { user: userInfo, defaultFirmaId } = response.data;

            setUserFirmalar(userInfo.firmalar || []);

            if (defaultFirmaId) {
                const defaultFirma = userInfo.firmalar?.find(f => f.firmaId === defaultFirmaId);
                setAktiveFirma(defaultFirma || null);
            } else if (userInfo.firmalar && userInfo.firmalar.length > 0) {
                setAktiveFirma(userInfo.firmalar[0]);
            }
        } catch (error) {
            console.error('Firma bilgileri yüklenirken hata:', error);
        } finally {
            setLoading(false);
        }
    };

    const changeFirma = async (firmaId) => {
        try {
            await userManagementApi.setDefaultFirma({ userId: user.id, yeniFirmaId: firmaId });
            const yeniFirma = userFirmalar.find(f => f.firmaId === firmaId);
            setAktiveFirma(yeniFirma);

            // Sayfayı yenile (veya state'i güncelle)
            window.location.reload();
        } catch (error) {
            console.error('Firma değiştirme hatası:', error);
        }
    };

    const hasYetki = (yetki) => {
        if (!aktiveFirma) return false;
        return aktiveFirma.yetkiler.includes(yetki) || aktiveFirma.yetkiler.includes('FirmaYoneticisi');
    };

    const value = {
        aktiveFirma,
        userFirmalar,
        loading,
        changeFirma,
        hasYetki
    };

    return <FirmaContext.Provider value={value}>{children}</FirmaContext.Provider>;
};

export const useFirma = () => {
    const context = useContext(FirmaContext);
    if (!context) {
        throw new Error('useFirma must be used within FirmaProvider');
    }
    return context;
};