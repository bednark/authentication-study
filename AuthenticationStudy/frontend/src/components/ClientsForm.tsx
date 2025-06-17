import { useState, useEffect } from "react";

import type { IClient } from "../lib/types";
import FloatingInput from "./FloatingInput";

interface IClientsFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IClient) => void;
  initialData?: IClient | null;
}

const ClientsForm = ({ isOpen, onClose, onSubmit, initialData }: IClientsFormProps) => {
  const [formData, setFormData] = useState<IClient>({
    id: 0,
    name: "",
    email: "",
    phone: "",
    address: "",
    city: "",
    postalCode: "",
    country: "",
  });

  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
    } else {
      setFormData({
        id: 0,
        name: "",
        email: "",
        phone: "",
        address: "",
        city: "",
        postalCode: "",
        country: "",
      });
    }
  }, [initialData]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onSubmit(formData);
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg p-8 w-full max-w-2xl shadow-lg">
        <h2 className="text-xl font-semibold mb-6 text-gray-900 dark:text-white">
          {initialData ? "Edytuj klienta" : "Dodaj nowego klienta"}
        </h2>
        <form onSubmit={handleSubmit}>
          <div className="grid md:grid-cols-2 md:gap-6">
            <FloatingInput name="name" label="Nazwa klienta" value={formData.name} onChange={handleChange} />
            <FloatingInput name="email" label="Email" value={formData.email} onChange={handleChange} type="email" />
            <FloatingInput name="phone" label="Telefon" value={formData.phone} onChange={handleChange} />
            <FloatingInput name="address" label="Adres" value={formData.address} onChange={handleChange} />
            <FloatingInput name="city" label="Miasto" value={formData.city} onChange={handleChange} />
            <FloatingInput name="postalCode" label="Kod pocztowy" value={formData.postalCode} onChange={handleChange} />
            <FloatingInput name="country" label="Kraj" value={formData.country} onChange={handleChange} />
          </div>
          <div className="flex justify-end mt-6 gap-3">
            <button
              type="button"
              onClick={onClose}
              className="text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg px-5 py-2.5 text-sm hover:bg-gray-100 dark:hover:bg-gray-700"
            >
              Anuluj
            </button>
            <button
              type="submit"
              className="text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            >
              {initialData ? "Zapisz zmiany" : "Dodaj"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ClientsForm;
