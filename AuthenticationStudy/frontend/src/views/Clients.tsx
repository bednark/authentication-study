import { useState, useEffect } from "react";

import { fetchClients, fetchClient, editClient,
  addClient, deleteClient } from "../lib/functions";
import type { IClient } from "../lib/types";

import ClientsTable from "../components/ClientsTable";
import Header from "../components/Header";
import Footer from "../components/Footer";
import ConfirmDeleteModal from "../components/ConfirmDeleteModal";
import ClientsForm from "../components/ClientsForm";
import LogoutBtn from "../components/LogoutBtn";

const Clients = () => {
  const [clients, setClients] = useState<IClient[]>([]);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [clientToEdit, setClientToEdit] = useState<IClient | null>(null);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState<boolean>(false);
  const [clientToDelete, setClientToDelete] = useState<IClient | null>(null);

  const loadClients = async () => {
    const data = await fetchClients();
    setClients(data);
  };

  useEffect(() => {
    loadClients();
  }, []);

  const handleDeleteClient = async (): Promise<void> => {
    if (clientToDelete) {
      await deleteClient(clientToDelete.id);
      setClients(prev => prev.filter(client => client.id !== clientToDelete?.id));
      setIsDeleteModalOpen(false);
    } else {
      console.warn("Client ID is not provided for deletion.");
    }
  };

  const handleOpenDeleteModal = (client: IClient): void => {
    setClientToDelete(client);
    setIsDeleteModalOpen(true);
  };

  const handleSave = async (data: IClient): Promise<void> => {
    if (clientToEdit !== null) {
      await editClient(data);
    } else {     
      await addClient(data);
    }
    await loadClients();
    handleCloseModal();
  };

  const handleOpenModal = async (clientId: number | null): Promise<void> => {
    if (!clientId) {
      setClientToEdit(null);
    } else {
      const client = await fetchClient(clientId);
      if (client) {
        setClientToEdit(client);
      }
      else {
        setClientToEdit(null);
        console.warn("Client not found");
      }
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = (): void => {
    setIsModalOpen(false);
    setClientToEdit(null);
  };

  return (
    <>
      <div className="container mx-auto p-4 bg-gray-900 min-w-screen h-screen
      flex flex-col justify-between">
        <div className="flex relative">
          <Header />
          <LogoutBtn />
        </div>
        <main>
          <ClientsTable clients={clients} handleOpenModal={handleOpenModal}
          handleOpenDeleteModal={handleOpenDeleteModal} />
          <div className="flex justify-end items mt-4">
            <button type="button" className="text-white bg-blue-700 hover:bg-blue-800
            font-medium rounded-lg text-sm px-5 py-2.5 me-2 mb-2 dark:bg-blue-600
            dark:hover:bg-blue-700 cursor-pointer"
            onClick={() => handleOpenModal(null)}>
              Dodaj klienta
            </button>
          </div>
        </main>
        <Footer />
      </div>

      <ClientsForm
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        onSubmit={(data) => handleSave(data)}
        initialData={clientToEdit}
      />
      <ConfirmDeleteModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleDeleteClient}
        clientName={clientToDelete?.name}
      />
    </>
  )
}

export default Clients;
