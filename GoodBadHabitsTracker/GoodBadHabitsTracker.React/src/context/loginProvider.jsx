import { createContext, useState } from 'react';

const LoginProvContext = createContext();

function LoginProvProvider({ children }) {
	const [loginProvider, setLoginProvider] = useState('');

	const setLoginProv = provider => {
		setLoginProvider(provider);
	};

	return <LoginProvContext.Provider value={{ loginProvider, setLoginProv }}>{children}</LoginProvContext.Provider>;
}

export { LoginProvProvider };
export default LoginProvContext;
