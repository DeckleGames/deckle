import { config } from '$lib/config';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch }) => {
  try {
    const response = await fetch(`${config.apiUrl}/projects`, {
      credentials: 'include'
    });

    if (response.ok) {
      const projects = await response.json();
      return { projects };
    }

    return { projects: [] };
  } catch (error) {
    console.error('Failed to load projects:', error);
    return { projects: [] };
  }
};
