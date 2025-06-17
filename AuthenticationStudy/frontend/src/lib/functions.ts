import axios from 'axios';
import type { IClient } from './types';

export const fetchClients = async (): Promise<IClient[]> => {
  try {
    const response = await axios.get('/api/clients');
    return response.data;
  } catch (error) {
    console.error("Error occured while fetching clients:", error);
    return [];
  }
}

export const fetchClient = async (id: number | null): Promise<IClient | null> => {
  if (id === null) {
    console.warn("Client ID is null, cannot fetch data.");
    return null;
  }

  try {
    const response = await axios.get(`/api/client/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error occured while fetching client ${id}:`, error);
    return null;
  }
}

export const editClient = async (client: IClient): Promise<IClient | null> => {
  try {
    const response = await axios.put(`/api/client/${client.id}`, client);
    return response.data;
  } catch (error) {
    console.error(`Error occured while editing client ${client.id}:`, error);
    return null;
  }
}

export const addClient = async (client: Omit<IClient, 'id'>): Promise<IClient | null> => {
  try {
    console.log("Adding client:", client);
    const response = await axios.post('/api/clients', client);
    return response.data;
  } catch (error) {
    console.error("Error occured while adding client:", error);
    return null;
  }
}

export const deleteClient = async (id: number): Promise<void> => {
  try {
    await axios.delete(`/api/client/${id}`);
  } catch (error) {
    console.error(`Error occured while deleting client ${id}:`, error);
  }
}

