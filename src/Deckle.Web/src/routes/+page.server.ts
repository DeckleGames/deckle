import { config } from '$lib/config';
import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch }) => {
  try {
    const response = await fetch(`${config.apiUrl}/auth/me`, {
      credentials: 'include'
    });

    if (response.ok) {
      throw redirect(302, '/projects');
    }
  } catch (error) {
    if (error instanceof Response && error.status === 302) {
      throw error;
    }
  }

  return {};
};
