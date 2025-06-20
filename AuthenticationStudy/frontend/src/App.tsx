import { Routes, Route } from "react-router-dom"

import Clients from "./views/Clients";
import Login from "./views/Login";
import axios from "axios";

const App = () => {
  const handleLogin = async (username: string, password: string) => {
    try {
      await axios.post("/api/auth/login", { username, password }, { withCredentials: true });
      window.location.href = "/klienci";
    } catch (err) {
      alert("Błędne dane logowania");
    }
  };

  return (
    <Routes>
      <Route path="/klienci" element={<Clients />} />
      <Route path="/login" element={
      <Login onLogin={handleLogin} />
      } />
    </Routes>
  );
}

export default App;
