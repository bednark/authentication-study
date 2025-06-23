import axios from "axios";

const LogoutBtn = () => {
  const handleLogout = async () => {
    await axios.post("/api/auth/logout");
  };

  return (
    <button type="button"
    className="focus:outline-none text-white bg-red-700
    hover:bg-red-800 focus:ring-4 focus:ring-red-300
    font-medium rounded-lg text-sm px-5 py-2.5
    me-2 mb-2 dark:bg-red-600 dark:hover:bg-red-700
    dark:focus:ring-red-900 absolute top-4 right-0"
    onClick={handleLogout}>
      Wyloguj się
    </button>
  )
}

export default LogoutBtn;
