import { Routes, Route, Navigate, useNavigate } from "react-router-dom"

import Clients from "./views/Clients";
import Login from "./views/Login";
import axios from "axios";

const App = () => {
  const navigate = useNavigate();

  const handleLogin = async (username: string, password: string) => {
    try {
      await axios.post("/api/auth/login", { username, password }, { withCredentials: true });
      navigate("/klienci");
    } catch (err) {
      alert("Błędne dane logowania");
    }
  };

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/klienci" />} />
      <Route path="/klienci" element={<Clients />} />
      <Route path="/login" element={
      <Login onLogin={handleLogin} />
      } />
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  );
}

export default App;
