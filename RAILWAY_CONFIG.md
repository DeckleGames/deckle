# Railway Configuration for deckle.games

## Custom Domain Setup

### DNS Configuration

Add the following CNAME records in your DNS provider (e.g., Cloudflare, Route53):

| Type  | Name | Value (Target)                          | TTL  |
|-------|------|-----------------------------------------|------|
| CNAME | api  | {provided-by-railway}.up.railway.app    | Auto |
| CNAME | app  | {provided-by-railway}.up.railway.app    | Auto |

**Note:** Railway will provide the exact CNAME target when you add the custom domain in each service's settings.

---

## API Service Environment Variables

Railway → API service → Variables

| Variable | Value | Notes |
|----------|-------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Required |
| `ASPNETCORE_URLS` | `http://+:8080` | Required |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | `true` | Required |
| `ConnectionStrings__deckledb` | `Host=${{Postgres.PGHOST}};Port=${{Postgres.PGPORT}};Username=${{Postgres.PGUSER}};Password=${{Postgres.PGPASSWORD}};Database=${{Postgres.PGDATABASE}}` | Use Railway reference |
| **`FrontendUrl`** | **`https://app.deckle.games`** | **CRITICAL - Must be set** |
| **`CookieDomain`** | **`.deckle.games`** | **Required for auth** (note the leading dot) |
| `Authentication__Google__ClientId` | `{your-client-id}.apps.googleusercontent.com` | From Google Cloud Console |
| `Authentication__Google__ClientSecret` | `GOCSPX-xxxxxxxxxxxxx` | From Google Cloud Console |
| `Email__SmtpHost` | `smtp.gmail.com` | Your SMTP provider |
| `Email__SmtpPort` | `587` | Usually 587 (TLS) or 465 (SSL) |
| `Email__UseSsl` | `true` | Match your SMTP configuration |
| `Email__Username` | `noreply@deckle.games` | Your SMTP username |
| `Email__Password` | `{your-smtp-password}` | SMTP password or app password |
| `Email__FromAddress` | `noreply@deckle.games` | Email "from" address |
| `Email__FromName` | `Deckle` | Email "from" name |

---

## Web Service Environment Variables

Railway → Web service → Variables

| Variable | Value | Notes |
|----------|-------|-------|
| `NODE_ENV` | `production` | Required |
| `PORT` | `5173` | Required |
| **`PUBLIC_API_URL`** | **`https://api.deckle.games`** | **Must match API custom domain** |
| **`ORIGIN`** | **`https://app.deckle.games`** | **Must match Web custom domain** |

---

## Railway Custom Domain Configuration

### API Service

1. Railway → API service → Settings → Networking → Custom Domains
2. Click "Add Custom Domain"
3. Enter: `api.deckle.games`
4. Railway will show you the CNAME target (e.g., `deckle-api-production-abc123.up.railway.app`)
5. Add this CNAME record to your DNS provider
6. Wait for DNS propagation (usually 5-30 minutes)
7. Railway will automatically provision SSL certificate via Let's Encrypt

### Web Service

1. Railway → Web service → Settings → Networking → Custom Domains
2. Click "Add Custom Domain"
3. Enter: `app.deckle.games`
4. Railway will show you the CNAME target
5. Add this CNAME record to your DNS provider
6. Wait for DNS propagation
7. Railway will automatically provision SSL certificate

---

## Google OAuth Configuration

Update your Google Cloud Console OAuth settings:

1. Go to: https://console.cloud.google.com/apis/credentials
2. Select your OAuth 2.0 Client ID
3. Under "Authorized redirect URIs", add:
   - `https://api.deckle.games/signin-google`
4. Remove or keep old Railway domain as backup:
   - `https://deckle-api.up.railway.app/signin-google`
5. Save changes

---

## Deployment Steps

1. **Add custom domains in Railway** (API and Web services)
2. **Update DNS records** with CNAME targets from Railway
3. **Wait for DNS propagation** (check with `nslookup api.deckle.games`)
4. **Update environment variables** in Railway (see tables above)
5. **Trigger redeploy** of both services (or push to master branch)
6. **Update Google OAuth** redirect URI
7. **Test authentication flow**:
   - Visit https://app.deckle.games
   - Click "Sign in with Google"
   - Should redirect to Google OAuth
   - Should redirect back to https://app.deckle.games/projects
   - Layout/TopBar should be visible

---

## Verification Checklist

- [ ] DNS CNAME records added for api.deckle.games and app.deckle.games
- [ ] Both domains show SSL certificate as active in Railway
- [ ] `FrontendUrl` set to `https://app.deckle.games` in API service
- [ ] `CookieDomain` set to `.deckle.games` in API service
- [ ] `PUBLIC_API_URL` set to `https://api.deckle.games` in Web service
- [ ] `ORIGIN` set to `https://app.deckle.games` in Web service
- [ ] Google OAuth redirect URI updated to `https://api.deckle.games/signin-google`
- [ ] Both services redeployed with new configuration
- [ ] Can successfully sign in with Google
- [ ] Layout/TopBar visible after authentication
- [ ] No CORS errors in browser console
- [ ] Authentication persists after page refresh

---

## Testing Commands

```bash
# Verify DNS propagation
nslookup api.deckle.games
nslookup app.deckle.games

# Test SSL certificates
curl -I https://api.deckle.games/health
curl -I https://app.deckle.games

# Check API responds
curl https://api.deckle.games/health

# View API logs in Railway
railway logs --service api

# View Web logs in Railway
railway logs --service web
```

---

## Troubleshooting

### DNS Not Resolving

- Wait up to 1 hour for DNS propagation
- Clear DNS cache: `ipconfig /flushdns` (Windows) or `sudo dscacheutil -flushcache` (Mac)
- Check DNS with: `dig api.deckle.games` or `nslookup api.deckle.games`

### SSL Certificate Not Provisioning

- Ensure DNS is resolving correctly first
- Railway auto-provisions SSL via Let's Encrypt (may take 5-10 minutes)
- Check Railway service logs for SSL provisioning errors

### CORS Errors Persist

- Verify `FrontendUrl` in API service matches exactly: `https://app.deckle.games` (no trailing slash)
- Check browser console for exact origin being blocked
- Verify both services have been redeployed after variable changes

### Authentication Cookie Issues

- Verify `CookieDomain` is set to `.deckle.games` (with leading dot)
- Check browser DevTools → Application → Cookies
- Cookie should be set for `.deckle.games` domain
- Cookie should have `Secure` and `HttpOnly` flags set

### OAuth Redirect Fails

- Verify Google OAuth redirect URI is exactly: `https://api.deckle.games/signin-google`
- Check API logs for OAuth errors
- Ensure `FrontendUrl` is set correctly in API service
