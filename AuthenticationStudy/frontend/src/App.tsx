import { Routes, Route, Navigate } from "react-router-dom"

import Clients from "./views/Clients";
import Login from "./views/Login";

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/klienci" />} />
      <Route path="/klienci" element={<Clients />} />
      <Route path="/login" element={
      <Login onLogin={(username, password) => {
        console.log("Użytkownik:", username, "Hasło:", password);
      }} />
      } />
    </Routes>
  );
}

export default App;
