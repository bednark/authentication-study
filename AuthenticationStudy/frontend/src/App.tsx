import { Routes, Route } from "react-router-dom"

import Clients from "./views/Clients";
import Login from "./views/Login";

const App = () => {
  return (
    <Routes>
      <Route path="/klienci" element={<Clients />} />
      <Route path="/login" element={
        <Login onLogin={(username, password) => {
          console.log("Użytkownik:", username, "Hasło:", password);
          // Tutaj możesz dodać żądanie do API
        }} />
      } />
    </Routes>
  );
}

export default App;
