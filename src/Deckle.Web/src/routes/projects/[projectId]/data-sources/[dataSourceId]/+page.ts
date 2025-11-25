import type { PageLoad } from './$types';

export const load: PageLoad = ({ params }) => {
  return {
    projectId: params.projectId,
    dataSourceId: params.dataSourceId
  };
};
