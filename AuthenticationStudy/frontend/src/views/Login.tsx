import { useState } from "react";

interface ILoginProps {
  onLogin: (username: string, password: string) => void;
}

const Login = ({ onLogin }: ILoginProps) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onLogin(username, password);
  };

  return (
    <div className="min-h-screen bg-gray-900 flex items-center justify-center px-4">
      <div className="bg-white dark:bg-gray-800 rounded-lg p-8 w-full max-w-md shadow-lg">
        <h1 className="text-2xl font-bold text-center text-gray-900 dark:text-white mb-6">
          Zaloguj się do panelu
        </h1>
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="username" className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
              Nazwa użytkownika
            </label>
            <input
              type="text"
              id="username"
              value={username}
              required
              onChange={(e) => setUsername(e.target.value)}
              className="w-full p-2.5 text-sm border rounded-lg bg-gray-50 border-gray-300 text-gray-900
                         focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600
                         dark:text-white dark:placeholder-gray-400"
              placeholder="np. admin"
            />
          </div>

          <div>
            <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
              Hasło
            </label>
            <input
              type="password"
              id="password"
              value={password}
              required
              onChange={(e) => setPassword(e.target.value)}
              className="w-full p-2.5 text-sm border rounded-lg bg-gray-50 border-gray-300 text-gray-900
                         focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600
                         dark:text-white dark:placeholder-gray-400"
              placeholder="••••••••"
            />
          </div>

          <button
            type="submit"
            className="w-full text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none
                       focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center
                       dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
          >
            Zaloguj się
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;
