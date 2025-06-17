const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white dark:bg-gray-900">
      <hr className="my-6 border-gray-200 sm:mx-auto dark:border-gray-700 lg:my-4" />
      <span className="block text-sm text-gray-500 sm:text-center dark:text-gray-400">© {currentYear} Jakub Bednarek</span>
    </footer>
  )
}

export default Footer;
