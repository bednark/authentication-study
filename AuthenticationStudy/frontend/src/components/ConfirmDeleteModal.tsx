interface IConfirmDeleteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
  clientName?: string;
}

const ConfirmDeleteModal = ({
  isOpen,
  onClose,
  onConfirm,
  clientName,
}: IConfirmDeleteModalProps) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg p-8 w-full max-w-md shadow-lg">
        <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
          Potwierdź usunięcie
        </h2>
        <p className="text-gray-700 dark:text-gray-300 mb-6">
          Czy na pewno chcesz usunąć klienta
          {clientName ? ` „${clientName}”` : ""}?
          Tej operacji nie można cofnąć.
        </p>
        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg px-5 py-2.5 text-sm hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            Anuluj
          </button>
          <button
            onClick={onConfirm}
            className="text-white bg-red-600 hover:bg-red-700 focus:ring-4 focus:outline-none focus:ring-red-300 font-medium rounded-lg text-sm px-5 py-2.5 dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800"
          >
            Usuń
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmDeleteModal;
