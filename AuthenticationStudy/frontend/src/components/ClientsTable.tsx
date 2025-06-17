import type { IClient } from '../lib/types';

interface IClientsTable {
  clients: IClient[];
  handleOpenModal?: (clientId: number | null) => void;
  handleOpenDeleteModal?: (client: IClient) => void;
}

const ClientsTable = ({ clients, handleOpenModal, handleOpenDeleteModal }: IClientsTable) => {
  return (
    <div className="overflow-auto max-h-[400px]">
      <table className="w-full text-sm text-left rtl:text-right text-gray-500">
        <thead className="text-xs text-gray-700 uppercase bg-gray-50 dark:bg-gray-700 dark:text-gray-400">
          <tr>
            <th scope="col" className="px-6 py-3">Nazwa klienta</th>
            <th scope="col" className="px-6 py-3">Email</th>
            <th scope="col" className="px-6 py-3">Telefon</th>
            <th scope="col" className="px-6 py-3">Adres</th>
            <th scope="col" className="px-6 py-3">Miasto</th>
            <th scope="col" className="px-6 py-3">Kod pocztowy</th>
            <th scope="col" className="px-6 py-3">Kraj</th>
            <th scope="col" className="px-6 py-3">Akcje</th>
          </tr>
        </thead>
        <tbody>
          {clients.map((client, index) => (
            <tr className="bg-white border-b dark:bg-gray-800 dark:border-gray-700 border-gray-200" key={index}>
              <th scope="row" className="px-6 py-4 font-medium text-gray-900 whitespace-nowrap dark:text-white">
                {client.name}
              </th>
              <td className="px-6 py-4">{client.email}</td>
              <td className="px-6 py-4">{client.phone}</td>
              <td className="px-6 py-4">{client.address}</td>
              <td className="px-6 py-4">{client.city}</td>
              <td className="px-6 py-4">{client.postalCode}</td>
              <td className="px-6 py-4">{client.country}</td>
              <td className="px-6 py-4 flex gap-4">
                <button type="button"
                className="hover:text-blue-500 text-blue-600
                text-sm cursor-pointer"
                onClick={() => handleOpenModal && handleOpenModal(client.id)}>
                  Edytuj
                </button>
                <button type="button" className="hover:text-red-500
                text-red-600 text-sm cursor-pointer"
                onClick={() => handleOpenDeleteModal && handleOpenDeleteModal(client)}>Usuń</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default ClientsTable;
