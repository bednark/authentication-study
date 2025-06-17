const Header = () => {
  return (
    <header className="flex flex-col items-center justify-center text-center text-white pt-24">
      <h1 className="mb-4 leading-none tracking-tight
      text-gray-900 text-xl lg:text-2xl dark:text-white">
        Porównanie wybranych metod autoryzacji API: JWT, OAuth2 i mTLS w kontekście bezpieczeństwa, logowania i reakcji na zagrożenia.  
      </h1>
      <p className="mb-6 font-normal text-gray-500
      sm:px-16 xl:px-48 dark:text-gray-400">
        Oto podstrona wyświetlająca listę klientów, która jest zabezpieczona za pomocą wybranych metod autoryzacji.
      </p>
    </header>
  )
}

export default Header;
