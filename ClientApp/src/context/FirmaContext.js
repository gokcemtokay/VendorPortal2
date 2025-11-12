import React, { createContext, useContext, useState, useEffect } from 'react';
import { firmaApi } from '../api/apiClient';
import { useAuth } from './AuthContext';

const FirmaContext = createContext();

export const useFirma = () => {
    const context = useContext(FirmaContext);
    if (!context) {
        throw new Error('useFirma must be used within FirmaProvider');
    }
    return context;
};

export const FirmaProvider = ({ children }) => {
    const { user, isAuthenticated } = useAuth();
    const [currentFirma, setCurrentFirma] = useState(null);
    const [firmalar, setFirmalar] = useState([]);
    const [loading, setLoading] = useState(false);

    // Kullanıcının firmasını yükle
    useEffect(() => {
        if (isAuthenticated && user?.firmaId) {
            loadUserFirma();
        }
    }, [isAuthenticated, user]);

    const loadUserFirma = async () => {
        if (!user?.firmaId) return;

        try {
            setLoading(true);
            const response = await firmaApi.getById(user.firmaId);
            if (response.data?.data) {
                setCurrentFirma(response.data.data);
            }
        } catch (error) {
            console.error('Firma yüklenirken hata:', error);
        } finally {
            setLoading(false);
        }
    };

    // Tüm firmaları yükle (Admin için)
    const loadAllFirmalar = async () => {
        try {
            setLoading(true);
            const response = await firmaApi.getAll();
            if (response.data?.data) {
                setFirmalar(response.data.data);
            }
        } catch (error) {
            console.error('Firmalar yüklenirken hata:', error);
        } finally {
            setLoading(false);
        }
    };

    // Firma seç
    const selectFirma = async (firmaId) => {
        try {
            const response = await firmaApi.getById(firmaId);
            if (response.data?.data) {
                setCurrentFirma(response.data.data);
                return response.data.data;
            }
        } catch (error) {
            console.error('Firma seçilirken hata:', error);
            throw error;
        }
    };

    // Firma oluştur
    const createFirma = async (firmaData) => {
        try {
            const response = await firmaApi.create(firmaData);
            if (response.data?.data) {
                // Firmalar listesini güncelle
                await loadAllFirmalar();
                return response.data.data;
            }
        } catch (error) {
            console.error('Firma oluşturulurken hata:', error);
            throw error;
        }
    };

    // Firma güncelle
    const updateFirma = async (firmaData) => {
        try {
            const response = await firmaApi.update(firmaData);
            if (response.data?.data) {
                // Eğer güncellenen firma mevcut firmaysa, güncelle
                if (currentFirma?.id === firmaData.id) {
                    setCurrentFirma(response.data.data);
                }
                // Firmalar listesini güncelle
                await loadAllFirmalar();
                return response.data.data;
            }
        } catch (error) {
            console.error('Firma güncellenirken hata:', error);
            throw error;
        }
    };

    // Firma sil
    const deleteFirma = async (firmaId) => {
        try {
            await firmaApi.delete(firmaId);
            // Firmalar listesini güncelle
            await loadAllFirmalar();
            // Eğer silinen firma mevcut firmaysa, temizle
            if (currentFirma?.id === firmaId) {
                setCurrentFirma(null);
            }
        } catch (error) {
            console.error('Firma silinirken hata:', error);
            throw error;
        }
    };

    const value = {
        currentFirma,
        firmalar,
        loading,
        loadUserFirma,
        loadAllFirmalar,
        selectFirma,
        createFirma,
        updateFirma,
        deleteFirma,
        // Helper functions
        isMusteriFirma: currentFirma?.firmaTipi === 1 || currentFirma?.firmaTipi === 3,
        isTedarikciFirma: currentFirma?.firmaTipi === 2 || currentFirma?.firmaTipi === 3,
    };

    return (
        <FirmaContext.Provider value={value}>
            {children}
        </FirmaContext.Provider>
    );
};

export default FirmaContext;