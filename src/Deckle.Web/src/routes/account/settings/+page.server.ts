import { authApi, mcpAccessTokensApi } from '$lib/api';
import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch }) => {
  try {
    const [user, tokens] = await Promise.all([
      authApi.me(fetch),
      mcpAccessTokensApi.list(fetch)
    ]);
    return { user, tokens };
  } catch (err) {
    console.error('Failed to load user data:', err);
    throw error(500, 'Failed to load user data');
  }
};
