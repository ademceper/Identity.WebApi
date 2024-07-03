
#### English Version

## Dream Identity Application

### Overview
The Dream Identity Application is a comprehensive identity management system that includes features such as user registration, login, password management, email and SMS verification, and role-based access control. This application is designed to be secure, scalable, and easy to integrate with other systems.

### Features
1. **User Registration and Login**: Users can register with their email and password. The system includes validation to prevent SQL and JavaScript injections.
2. **Password Management**: Users can change their password and reset it if forgotten. A verification code is sent via email for password reset.
3. **Email and SMS Verification**: Users receive a verification code via email or SMS for additional security during login.
4. **Role-Based Access Control**: Admin and user roles are supported, allowing for different levels of access.
5. **Rate Limiting**: The application limits the number of requests to protect against DDOS attacks.
6. **Profile Management**: Users can update their profile information.
7. **Audit Logs**: The application logs user activities for security and compliance purposes.

### Technologies Used
- **.NET Core**: Backend framework
- **Entity Framework Core**: ORM for database access
- **Identity Framework**: For authentication and authorization
- **FluentValidation**: For input validation
- **AutoMapper**: For object-to-object mapping
- **SMTP**: For sending emails
- **SMS Service**: For sending SMS messages

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/dream-identity-app.git
   ```
2. Navigate to the project directory:
   ```bash
   cd dream-identity-app
   ```
3. Restore the dependencies:
   ```bash
   dotnet restore
   ```
4. Update the `appsettings.json` file with your configuration settings, including database connection strings and email/SMS service credentials.

### Running the Application
1. Build the project:
   ```bash
   dotnet build
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. The application will be available at `https://localhost:5001`.

### Usage
- **Register a New User**: Send a POST request to `/api/auth/register` with the user details.
- **Login**: Send a POST request to `/api/auth/login` with the user's email and password.
- **Forgot Password**: Send a POST request to `/api/auth/forgot-password` with the user's email.
- **Reset Password**: Send a POST request to `/api/auth/reset-password` with the verification code and new password.
- **Send Login Code**: Send a POST request to `/api/auth/send-login-code` with the user's email.
- **Verify Login Code**: Send a POST request to `/api/auth/verify-login-code` with the verification code.
- **Update Profile**: Send a PUT request to `/api/auth/update-profile` with the updated user details.

### Contributing
1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

### License
This project is licensed under the MIT License. See the LICENSE file for details.

---

#### Türkçe Sürüm

## Dream Kimlik Uygulaması

### Genel Bakış
Dream Kimlik Uygulaması, kullanıcı kaydı, giriş, şifre yönetimi, e-posta ve SMS doğrulama, rol tabanlı erişim kontrolü gibi özellikler içeren kapsamlı bir kimlik yönetim sistemidir. Bu uygulama, güvenli, ölçeklenebilir ve diğer sistemlerle kolayca entegre edilebilir olacak şekilde tasarlanmıştır.

### Özellikler
1. **Kullanıcı Kaydı ve Girişi**: Kullanıcılar e-posta ve şifreleri ile kayıt olabilirler. Sistem, SQL ve JavaScript enjeksiyonlarına karşı doğrulama içerir.
2. **Şifre Yönetimi**: Kullanıcılar şifrelerini değiştirebilir ve unuturlarsa sıfırlayabilirler. Şifre sıfırlama için e-posta ile bir doğrulama kodu gönderilir.
3. **E-posta ve SMS Doğrulama**: Kullanıcılar, giriş sırasında ek güvenlik için e-posta veya SMS ile bir doğrulama kodu alır.
4. **Rol Tabanlı Erişim Kontrolü**: Yönetici ve kullanıcı rolleri desteklenir, bu sayede farklı erişim seviyeleri sağlanır.
5. **Hız Sınırlama**: Uygulama, DDOS saldırılarına karşı koruma sağlamak için istek sayısını sınırlar.
6. **Profil Yönetimi**: Kullanıcılar profil bilgilerini güncelleyebilirler.
7. **Aktivite Kayıtları**: Uygulama, kullanıcı aktivitelerini güvenlik ve uyumluluk amacıyla kaydeder.

### Kullanılan Teknolojiler
- **.NET Core**: Backend framework
- **Entity Framework Core**: Veritabanı erişimi için ORM
- **Identity Framework**: Kimlik doğrulama ve yetkilendirme için
- **FluentValidation**: Girdi doğrulama için
- **AutoMapper**: Nesne-nesne haritalama için
- **SMTP**: E-posta göndermek için
- **SMS Hizmeti**: SMS mesajları göndermek için

### Kurulum
1. Depoyu klonlayın:
   ```bash
   git clone https://github.com/yourusername/dream-identity-app.git
   ```
2. Proje dizinine gidin:
   ```bash
   cd dream-identity-app
   ```
3. Bağımlılıkları yükleyin:
   ```bash
   dotnet restore
   ```
4. `appsettings.json` dosyasını yapılandırma ayarları ile güncelleyin, veritabanı bağlantı dizeleri ve e-posta/SMS hizmeti kimlik bilgileri dahil.

### Uygulamayı Çalıştırma
1. Projeyi derleyin:
   ```bash
   dotnet build
   ```
2. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```
3. Uygulama `https://localhost:5001` adresinde kullanılabilir olacaktır.

### Kullanım
- **Yeni Kullanıcı Kaydı**: Kullanıcı detayları ile `/api/auth/register` adresine POST isteği gönderin.
- **Giriş**: Kullanıcının e-posta ve şifresi ile `/api/auth/login` adresine POST isteği gönderin.
- **Şifremi Unuttum**: Kullanıcının e-postası ile `/api/auth/forgot-password` adresine POST isteği gönderin.
- **Şifre Sıfırlama**: Doğrulama kodu ve yeni şifre ile `/api/auth/reset-password` adresine POST isteği gönderin.
- **Giriş Kodu Gönderme**: Kullanıcının e-postası ile `/api/auth/send-login-code` adresine POST isteği gönderin.
- **Giriş Kodunu Doğrulama**: Doğrulama kodu ile `/api/auth/verify-login-code` adresine POST isteği gönderin.
- **Profili Güncelleme**: Güncellenmiş kullanıcı detayları ile `/api/auth/update-profile` adresine PUT isteği gönderin.

### Katkıda Bulunma
1. Depoyu fork edin.
2. Yeni bir dal oluşturun (`git checkout -b feature-branch`).
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik ekle'`).
4. Dalı push edin (`git push origin feature-branch`).
5. Bir Pull Request açın.

### Lisans
Bu proje MIT Lisansı altında lisanslanmıştır. Detaylar için LICENSE dosyasına bakın.
