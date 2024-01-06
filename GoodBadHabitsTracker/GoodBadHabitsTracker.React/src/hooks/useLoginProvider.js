import { useContext } from 'react';
import LoginProvContext from '../context/loginProvider';

export default function useLoginProvider() {
	return useContext(LoginProvContext);
}
