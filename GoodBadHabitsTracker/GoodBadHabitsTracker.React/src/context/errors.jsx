import { createContext, useState } from 'react';

const ErrorsContext = createContext();

function ErrorsProvider({ children }) {
	const [errors, setErrors] = useState('');

	const resetErrors = () => {
		setErrors('');
	};

	return <ErrorsContext.Provider value={{errors, resetErrors}}>{children}</ErrorsContext.Provider>;
}

export { ErrorsProvider };
export default ErrorsContext;
