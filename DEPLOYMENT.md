# Deckle Railway Deployment Guide

This guide provides step-by-step instructions for deploying Deckle to Railway using pre-built Docker images from GitHub Container Registry (GHCR).

**Note:** For a complete custom domain setup example with deckle.games, see [RAILWAY_CONFIG.md](RAILWAY_CONFIG.md).

## Prerequisites

- Railway account (sign up at https://railway.app)
- Railway CLI installed (optional but recommended): `npm install -g @railway/cli`
- GitHub Personal Access Token with `read:packages` scope
- Access to project secrets (Google OAuth, SMTP credentials)

## Architecture Overview

```
GitHub Actions (CI/CD)
    ↓
Build & Push to GHCR
    ↓
Trigger Railway Webhooks
    ↓
Railway Project
├── PostgreSQL Database (Railway managed)
├── API Service (pulls ghcr.io/.../deckle-api)
└── Web Service (pulls ghcr.io/.../deckle-web)
```

## Step 1: Create Railway Project

### Option A: Using Railway CLI

```bash
# Login to Railway
railway login

# Create new project
railway init

# Link to existing Railway project (if already created)
railway link
```

### Option B: Using Railway Dashboard

1. Go to https://railway.app/dashboard
2. Click "New Project"
3. Select "Empty Project"
4. Name your project (e.g., "deckle-production")

## Step 2: Add PostgreSQL Database

### Using Railway CLI

```bash
railway add --database postgres
```

### Using Railway Dashboard

1. In your project, click "+ New"
2. Select "Database"
3. Choose "Add PostgreSQL"
4. Railway will automatically provision the database and create a `DATABASE_URL` variable

## Step 3: Configure API Service

### 3.1 Create API Service

**Using Railway Dashboard:**
1. Click "+ New" → "Empty Service"
2. Name it "api"
3. Go to service Settings

**Configure Docker Image Source:**
1. Settings → Source → "Docker Image"
2. Image: `ghcr.io/{your-github-username}/{repo-name}/deckle-api:latest`
   - Replace with your actual GHCR path
   - Example: `ghcr.io/johndoe/deckle/deckle-api:latest`

### 3.2 Add GHCR Authentication

Railway needs credentials to pull private images from GHCR.

**Create GitHub Personal Access Token:**
1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Generate new token
3. Select scope: `read:packages`
4. Copy the token (you won't see it again)

**Add Registry Credentials in Railway:**
1. API service → Settings → Source
2. Under "Image Registry Credentials":
   - Registry: `ghcr.io`
   - Username: Your GitHub username
   - Password: PAT from above

### 3.3 Configure API Environment Variables

Go to API service → Variables and add:

#### Required Variables

| Variable | Value | Description |
|----------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | ASP.NET environment |
| `ASPNETCORE_URLS` | `http://+:8080` | Listening URL |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | `true` | Enable proxy headers |
| `ConnectionStrings__deckledb` | `Host=${{Postgres.PGHOST}};Port=${{Postgres.PGPORT}};Username=${{Postgres.PGUSER}};Password=${{Postgres.PGPASSWORD}};Database=${{Postgres.PGDATABASE}}` | Database connection (Railway reference) |
| `FrontendUrl` | `https://${{web.RAILWAY_PUBLIC_DOMAIN}}` | **REQUIRED**: Frontend URL (Railway reference) |
| `CookieDomain` | _(see note below)_ | **OPTIONAL**: Cookie domain for cross-domain auth |

**IMPORTANT - Cookie Domain Configuration:**

Railway default domains (`*.up.railway.app`) cannot share cookies between services because browsers don't allow cookie sharing across different subdomains of a public suffix. This means authentication cookies set by the API won't be accessible to the Web frontend.

**Options:**

1. **Use a Custom Domain (Recommended)**:
   - Set up custom subdomains: `api.yourdomain.com` and `app.yourdomain.com`
   - Example: `api.deckle.games` and `app.deckle.games`
   - Set `CookieDomain` to `.yourdomain.com` (note the leading dot)
   - Example: `.deckle.games`
   - This allows both subdomains to share authentication cookies
   - See Step 8 for custom domain setup or [RAILWAY_CONFIG.md](RAILWAY_CONFIG.md) for complete example

2. **Use Railway Default Domains (Limited)**:
   - Leave `CookieDomain` empty or unset
   - Authentication will work, but cookies won't persist across page refreshes due to cross-domain limitations
   - Not recommended for production use

#### Authentication - Google OAuth

| Variable | Value | Where to Get |
|----------|-------|--------------|
| `Authentication__Google__ClientId` | `your-client-id.apps.googleusercontent.com` | Google Cloud Console → APIs & Services → Credentials |
| `Authentication__Google__ClientSecret` | `GOCSPX-xxxxxxxxxxxxx` | Same as above |

**Important:** After deploying, you must add Railway's domain to Google OAuth redirect URIs:
- `https://{your-api-domain}.railway.app/api/auth/google-callback`

#### Email - SMTP Configuration

| Variable | Value | Example |
|----------|-------|---------|
| `Email__SmtpHost` | SMTP server hostname | `smtp.gmail.com` |
| `Email__SmtpPort` | SMTP port | `587` (TLS) or `465` (SSL) |
| `Email__UseSsl` | `true` or `false` | `true` |
| `Email__Username` | SMTP username | `noreply@yourdomain.com` |
| `Email__Password` | SMTP password | Your SMTP password or app password |
| `Email__FromAddress` | From email address | `noreply@yourdomain.com` |
| `Email__FromName` | From display name | `Deckle` |

**Gmail Users:** Use App Passwords (not your regular password)
- Google Account → Security → 2-Step Verification → App passwords

### 3.4 Configure Service Settings

**Port Configuration:**
1. Settings → Networking
2. Port: `8080` (matches ASPNETCORE_URLS)

**Health Check (Optional):**
1. Settings → Deploy
2. Health check path: `/health` or `/swagger`

**Restart Policy:**
1. Settings → Deploy
2. Restart policy: "On Failure"
3. Max retries: 10

## Step 4: Configure Web Service

### 4.1 Create Web Service

**Using Railway Dashboard:**
1. Click "+ New" → "Empty Service"
2. Name it "web"
3. Go to service Settings

**Configure Docker Image Source:**
1. Settings → Source → "Docker Image"
2. Image: `ghcr.io/{your-github-username}/{repo-name}/deckle-web:latest`

**Add GHCR Authentication:** (same as API service)
1. Settings → Source → Image Registry Credentials
2. Registry: `ghcr.io`
3. Username: Your GitHub username
4. Password: PAT from Step 3.2

### 4.2 Configure Web Environment Variables

Go to Web service → Variables and add:

| Variable | Value | Description |
|----------|-------|-------------|
| `NODE_ENV` | `production` | Node.js environment |
| `PORT` | `5173` | SvelteKit listening port |
| `PUBLIC_API_URL` | `https://${{api.RAILWAY_PUBLIC_DOMAIN}}` | API URL for frontend |
| `ORIGIN` | `https://${{web.RAILWAY_PUBLIC_DOMAIN}}` | Frontend origin |

### 4.3 Enable Public Domain

**Generate Railway Domain:**
1. Settings → Networking
2. Under "Public Networking", click "Generate Domain"
3. Railway will create: `{service-name}-production-{hash}.up.railway.app`
4. Copy this URL - you'll need it

**Port Configuration:**
1. Networking → Port: `5173`

## Step 5: GitHub Actions Integration

The workflow is already configured to trigger Railway deployments automatically.

### 5.1 Create Railway Webhooks

**For API Service:**
1. Railway → API service → Settings
2. Scroll to "Webhooks"
3. Click "Create Webhook"
4. Copy the webhook URL

**For Web Service:**
1. Railway → Web service → Settings
2. Create webhook
3. Copy the webhook URL

**Webhook URL Format:**
```
https://railway.app/project/{project-id}/service/{service-id}/webhook
```

### 5.2 Add Webhooks to GitHub Secrets

1. Go to your GitHub repository
2. Settings → Secrets and variables → Actions → Repository secrets
3. Add two new secrets:

| Secret Name | Value |
|-------------|-------|
| `RAILWAY_API_WEBHOOK_URL` | API webhook URL from Railway |
| `RAILWAY_WEB_WEBHOOK_URL` | Web webhook URL from Railway |

### 5.3 How Automatic Deployment Works

When you push to the `master` branch:

1. GitHub Actions builds the project
2. Runs tests
3. Builds Docker images
4. Pushes images to GHCR with tags: `:latest`, `:1.0.0`, `:1.0`, `:1`, `:abc1234`
5. **Triggers Railway webhooks** (new step)
6. Railway pulls `:latest` images from GHCR
7. Redeploys API and Web services

## Step 6: First Deployment

### Initial Manual Deployment

Before automatic deployments work, trigger the first deployment manually:

**Option A: Via Railway Dashboard**
1. Go to API service
2. Click "Deploy" → "Trigger Deploy"
3. Repeat for Web service

**Option B: Via Railway CLI**
```bash
railway up --service api
railway up --service web
```

### Monitor Deployment

**View Logs:**
```bash
# Railway CLI
railway logs --service api
railway logs --service web

# Or in Railway Dashboard
# Service → Deployments → Latest deployment → View logs
```

**Check Deployment Status:**
- Green checkmark = successful
- Red X = failed (click for error details)

## Step 7: Verify Deployment

### 7.1 API Health Check

Visit your API domain:
```
https://{your-api-domain}.railway.app/health
```

**Expected Response:** `200 OK` or health status JSON

### 7.2 API Documentation

Visit Scalar OpenAPI docs:
```
https://{your-api-domain}.railway.app/scalar/v1
```

Should display interactive API documentation.

### 7.3 Web Application

Visit your web domain:
```
https://{your-web-domain}.railway.app
```

The Deckle frontend should load.

### 7.4 Database Migrations

Check API logs for migration messages:
```bash
railway logs --service api
```

Look for Entity Framework migration logs. The database schema is applied automatically on startup.

### 7.5 End-to-End Test

1. Open web application
2. Click "Sign in with Google"
3. Complete OAuth flow
4. Create a test project
5. Verify it's saved (check PostgreSQL)

## Step 8: Custom Domain (Required for Production)

**Important:** Custom domains are required for proper authentication in production. Railway default domains cannot share cookies between services.

**Example Setup:** `api.deckle.games` and `app.deckle.games`

### 8.1 Add Custom Domain to Both Services

**API Service:**
1. Railway → API service → Settings → Networking → Custom Domains
2. Click "Add Custom Domain"
3. Enter your API domain: `api.yourdomain.com` (e.g., `api.deckle.games`)
4. Railway will provide a CNAME target (e.g., `deckle-api-production-xyz123.up.railway.app`)
5. Copy this target - you'll add it to DNS

**Web Service:**
1. Railway → Web service → Settings → Networking → Custom Domains
2. Click "Add Custom Domain"
3. Enter your web domain: `app.yourdomain.com` (e.g., `app.deckle.games`)
4. Railway will provide a CNAME target
5. Copy this target - you'll add it to DNS

### 8.2 Configure DNS

Add CNAME records in your DNS provider (Cloudflare, Route53, etc.):

```
Type: CNAME
Name: api
Value: {api-cname-from-railway}.up.railway.app

Type: CNAME
Name: app
Value: {web-cname-from-railway}.up.railway.app
```

**Example for deckle.games:**
```
Type: CNAME
Name: api
Value: deckle-api-production-abc123.up.railway.app

Type: CNAME
Name: app
Value: deckle-web-production-xyz789.up.railway.app
```

**Wait for DNS propagation** (5-30 minutes). Verify with:
```bash
nslookup api.yourdomain.com
nslookup app.yourdomain.com
```

### 8.3 Update Environment Variables

After DNS is propagated and domains are active in Railway:

**API Service Variables:**

| Variable | Value | Example (deckle.games) |
|----------|-------|------------------------|
| `FrontendUrl` | `https://app.yourdomain.com` | `https://app.deckle.games` |
| `CookieDomain` | `.yourdomain.com` (with leading dot) | `.deckle.games` |

**Web Service Variables:**

| Variable | Value | Example (deckle.games) |
|----------|-------|------------------------|
| `PUBLIC_API_URL` | `https://api.yourdomain.com` | `https://api.deckle.games` |
| `ORIGIN` | `https://app.yourdomain.com` | `https://app.deckle.games` |

**Google OAuth Configuration:**

1. Go to Google Cloud Console → APIs & Services → Credentials
2. Edit your OAuth 2.0 Client ID
3. Update authorized redirect URI to:
   - `https://api.yourdomain.com/signin-google`
   - Example: `https://api.deckle.games/signin-google`
4. Save changes

### 8.4 SSL Certificate

Railway automatically provisions SSL certificates for custom domains (via Let's Encrypt).

## Troubleshooting

### Deployment Fails with "Image Pull Error"

**Cause:** Railway cannot authenticate with GHCR.

**Solution:**
1. Verify GHCR credentials in service settings
2. Ensure PAT has `read:packages` scope
3. Check image path is correct (lowercase)

### Database Connection Errors

**Symptoms:** API fails to start, logs show "Unable to connect to database" or similar errors.

**Check:**
1. **Verify Service Reference Name:**
   - The service name in `${{Postgres.DATABASE_URL}}` must match your PostgreSQL service name exactly (case-sensitive)
   - Check Railway dashboard → PostgreSQL service card for the exact name
   - If named "postgres" (lowercase), use `${{postgres.DATABASE_URL}}`
   - If named something else, use `${{YourServiceName.DATABASE_URL}}`

2. **Verify Connection String Format:**
   - In Railway dashboard, go to Postgres service → Variables
   - Copy the `DATABASE_URL` value
   - It should look like: `postgresql://postgres:PASSWORD@postgres.railway.internal:5432/railway`
   - The reference `${{Postgres.DATABASE_URL}}` should resolve to this full connection string

3. **Check Service Status:**
   - PostgreSQL service is running (green status indicator)
   - No recent crashes or restarts

4. **Check Service Dependencies:**
   - API service → Settings → Deploy
   - Ensure Postgres is listed as a dependency (this ensures Postgres starts before API)

### Authentication Issues / Redirect to "projects/"

**Symptoms:**
- After signing in with Google, redirected to "projects/" (relative URL with no domain)
- CORS errors when making API requests from the web app
- Layout/TopBar not showing after authentication

**Root Cause:** `FrontendUrl` environment variable is not configured in the API service.

**Solution:**
1. Go to Railway → API service → Variables
2. Verify `FrontendUrl` is set to: `https://${{web.RAILWAY_PUBLIC_DOMAIN}}`
3. If missing, add it and redeploy the API service
4. Check API logs for: `"Auth login initiated. Redirecting to: https://..."`
5. If you see warnings about FrontendUrl, the variable is not being resolved correctly

**Additional Cookie/Session Issues:**

If authentication works but doesn't persist (layout doesn't show, re-authentication required):

**Cause:** Railway default domains can't share cookies between services.

**Solution:**
- Set up custom domain with subdomains (recommended - see Step 8)
- Configure `CookieDomain` to `.yourdomain.com` in API service variables

### Google OAuth Fails

**Check:**
1. `Authentication__Google__ClientId` and `ClientSecret` are correct
2. Redirect URI added to Google Cloud Console:
   - `https://{api-domain}/signin-google`
3. OAuth consent screen configured

### Web Can't Reach API

**Check:**
1. `PUBLIC_API_URL` uses correct Railway reference: `${{api.RAILWAY_PUBLIC_DOMAIN}}`
2. API service has public networking enabled
3. API is listening on correct port (8080)

### Email Not Sending

**Check:**
1. All `Email__*` variables are set
2. SMTP credentials are correct
3. SMTP port is correct (587 for TLS, 465 for SSL)
4. Firewall allows outbound SMTP connections

## Cost Estimates

**Railway Pricing (Hobby Plan):**
- $5 per service per month (usage-based)
- PostgreSQL: $5/month minimum
- Estimated total: $15-20/month (API + Web + PostgreSQL)

**Free Tier:**
- $5 free credits per month
- Suitable for development/testing
- Services pause when credits exhausted

**To reduce costs:**
- Use single environment (no staging)
- Scale down replicas to 1
- Set sleep policy for non-critical services

## Monitoring & Maintenance

### View Logs

**Real-time:**
```bash
railway logs --service api --follow
railway logs --service web --follow
```

**In Dashboard:**
- Service → Observability → Logs

### Metrics

Railway provides built-in metrics:
- CPU usage
- Memory usage
- Network traffic
- Request counts

Access: Service → Observability → Metrics

### Database Backups

Railway PostgreSQL includes:
- Automatic daily backups (retained 7 days)
- Point-in-time recovery
- Manual backup/restore via Railway CLI

**Manual backup:**
```bash
railway run pg_dump > backup.sql
```

### Rollback Deployment

If a deployment breaks production:

**Via Dashboard:**
1. Service → Deployments
2. Find previous successful deployment
3. Click "Redeploy"

**Via CLI:**
```bash
railway up --service api --image ghcr.io/.../deckle-api:1.0.0
```

## Security Best Practices

1. **Secrets Management:**
   - Never commit secrets to Git
   - Use Railway's environment variables (encrypted at rest)
   - Rotate SMTP passwords and OAuth secrets regularly

2. **GHCR Access:**
   - Use minimal scope PAT (`read:packages` only)
   - Rotate PAT every 6-12 months
   - Store securely in Railway (not in code)

3. **Database:**
   - Railway PostgreSQL uses SSL by default
   - Database is private (Railway services only)
   - Enable connection pooling for production

4. **HTTPS:**
   - Railway provides automatic SSL/TLS
   - All endpoints use HTTPS
   - Cookies are Secure and HttpOnly

5. **Rate Limiting:**
   - Consider adding rate limiting middleware to API
   - Use Railway's built-in DDoS protection

## Support Resources

- **Railway Documentation:** https://docs.railway.app
- **Railway Discord:** https://discord.gg/railway
- **Railway Status:** https://status.railway.app
- **GitHub Actions Logs:** Repository → Actions tab

## Summary Checklist

Before going live:

- [ ] Railway project created with PostgreSQL
- [ ] API service configured with GHCR image and environment variables
- [ ] Web service configured with GHCR image and environment variables
- [ ] GHCR authentication added to both services
- [ ] GitHub secrets added (`RAILWAY_API_WEBHOOK_URL`, `RAILWAY_WEB_WEBHOOK_URL`)
- [ ] Google OAuth redirect URI updated with Railway domain
- [ ] First manual deployment successful
- [ ] API health check returns 200 OK
- [ ] Web application loads and connects to API
- [ ] Database migrations applied successfully
- [ ] End-to-end test completed (sign in, create project)
- [ ] Custom domain configured (if applicable)
- [ ] SSL certificate active
- [ ] Monitoring and alerts configured

## Next Steps After Deployment

1. **Set up monitoring:** Configure alerting for downtime
2. **Backup strategy:** Schedule regular database exports
3. **Performance tuning:** Monitor metrics and optimize queries
4. **Scale as needed:** Increase replicas or upgrade service plans
5. **CI/CD improvements:** Add staging environment, blue-green deployments

---

For questions or issues, refer to the Railway documentation or open a GitHub issue in the Deckle repository.
