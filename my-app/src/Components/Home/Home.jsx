import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Home.css';
import AOS from 'aos';
import 'aos/dist/aos.css';

const HomePage = () => {
  const navigate = useNavigate();

  useEffect(() => {
    AOS.init({
      duration: 1000,
      once: true
    });
  }, []);

  const handleLoginClick = () => {
    navigate('/login');
  };

  return (
    <div className="commercial-auto-page">
      {/* Navbar */}
      <nav className="navbar navbar-expand-lg navbar-dark fixed-top" style={{ backgroundColor: '#1a2a5a' }}>
        <div className="container-fluid">
          <a className="navbar-brand fw-bold" href="#">
            
            InsureWise
          </a>
          <button 
            className="navbar-toggler" 
            type="button" 
            data-bs-toggle="collapse" 
            data-bs-target="#navbarNav"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
              <li className="nav-item"><a className="nav-link" href="#home">Home</a></li>
              <li className="nav-item"><a className="nav-link" href="#coverage">Coverage</a></li>
              <li className="nav-item"><a className="nav-link" href="#benefits">Benefits</a></li>
              <li className="nav-item"><a className="nav-link" href="#contact">Contact</a></li>
            </ul>
            <button className="btn btn-light" onClick={handleLoginClick}>Login / Register</button>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="hero-section" id="home" >
        <div className="container h-100 d-flex align-items-center">
          <div className="row align-items-center">
            <div className="col-lg-6 text-black" data-aos="fade-right">
              <h1 className="display-4 fw-bold mb-4">Commercial Auto Insurance Solutions</h1>
              <p className="lead mb-4">Protecting your business vehicles with comprehensive coverage tailored to your needs</p>
            </div>
            
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="py-5 bg-light">
        <div className="container">
          <div className="row g-4 text-center">
            {[
              { number: "10,000+", label: "Businesses Protected" },
              { number: "₹50Cr+", label: "Claims Paid Annually" },
              { number: "24/7", label: "Claim Support" },
              { number: "500+", label: "Network Garages" }
            ].map((stat, index) => (
              <div className="col-md-3" key={index} data-aos="fade-up" data-aos-delay={index * 100}>
                <div className="p-4 stat-card bg-white rounded shadow-sm">
                  <h2 className="fw-bold mb-0" style={{ color: '#1a2a5a' }}>{stat.number}</h2>
                  <p className="mb-0 text-muted">{stat.label}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>
       {/* Coverage Section */}
       <section id="coverage" className="py-6 bg-light position-relative">
        <div className="container">
          <div className="text-center mb-6" data-aos="fade-up">
            <h2 className="display-5 fw-bold mb-3">Comprehensive Coverage Options</h2>
            <p className="lead text-muted">Tailored protection for your commercial vehicles</p>
          </div>
          
          <div className="row g-4">
            {[
              { 
                title: "Liability Coverage", 
                description: "Protection against third-party injuries and property damage",
                icon: "🛡️",
                bgClass: "bg-danger bg-opacity-10"
              },
              { 
                title: "Physical Damage", 
                description: "Covers damage to your vehicles from collisions and other incidents",
                icon: "🚛",
                bgClass: "bg-warning bg-opacity-10"
              },
              { 
                title: "Cargo Insurance", 
                description: "Protection for goods being transported in your vehicles",
                icon: "📦",
                bgClass: "bg-info bg-opacity-10"
              },
              { 
                title: "Non-Trucking Liability", 
                description: "Coverage when trucks are used for non-business purposes",
                icon: "🚚",
                bgClass: "bg-success bg-opacity-10"
              }
            ].map((item, index) => (
              <div className="col-md-6 col-lg-3" key={index} data-aos="fade-up" data-aos-delay={index * 100}>
                <div className={`card h-100 border-0 shadow-sm ${item.bgClass}`}>
                  <div className="card-body text-center p-4">
                    <div className="icon-wrapper mb-4">
                      <span className="display-4">{item.icon}</span>
                    </div>
                    <h5 className="card-title fw-bold">{item.title}</h5>
                    <p className="card-text">{item.description}</p>
                  </div>
                </div>
                </div>
            ))}
          </div>
        </div>
      </section>
      {/* Benefits Section with Image */}
      <section id="benefits" className="py-6 benefits-section">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-lg-6" data-aos="fade-left">
              <h2 className="display-5 fw-bold mb-4">Why Choose Our Commercial Auto Insurance?</h2>
              <div className="benefits-list">
                {[
                  "Customized policies for fleets of all sizes",
                  "Competitive rates with flexible payment options",
                  "24/7 claims service with fast response times",
                  "Industry-specific coverage options",
                  "Dedicated account managers for your business",
                  "Discounts for safety programs and driver training"
                ].map((benefit, index) => (
                  <div className="d-flex mb-3" key={index}>
                    <div className="me-3 text-primary">
                      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9 12L11 14L15 10M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      </svg>
                    </div>
                    <p className="mb-0 fs-5">{benefit}</p>
                  </div>
                ))}
              </div>
              <button className="btn btn-primary mt-3 px-3 py-2 fw-bold" onClick={handleLoginClick}>Get Started</button>
              <br></br>
            </div>
          </div>
        </div>
      </section>

      {/* Testimonials */}
      <section className="py-5">
        <div className="container">
          <div className="text-center mb-5" data-aos="fade-up">
            <h2 className="fw-bold mb-3" style={{ color: '#1a2a5a' }}>What Our Clients Say</h2>
            <p className="lead text-muted">Trusted by businesses across India</p>
          </div>
          
          <div className="row g-4">
            {[
              {
                quote: "InsureWise handled our fleet claim efficiently when one of our trucks was involved in an accident. Their support team was available 24/7.",
                name: "Rajesh Kumar",
                company: "Speedy Logistics",
                rating: 5
              },
              {
                quote: "The customized policy for our construction vehicles saved us thousands in premiums while maintaining excellent coverage.",
                name: "Priya Sharma",
                company: "Sharma Builders",
                rating: 4
              }
            ].map((testimonial, index) => (
              <div className="col-lg-6" key={index} data-aos="fade-up" data-aos-delay={index * 100}>
                <div className="card h-100 border-0 shadow-sm">
                  <div className="card-body p-4">
                    <div className="mb-4">
                      {[...Array(testimonial.rating)].map((_, i) => (
                        <svg key={i} width="24" height="24" viewBox="0 0 24 24" fill="#FFD700" xmlns="http://www.w3.org/2000/svg">
                          <path d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z" stroke="#FFD700" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                      ))}
                    </div>
                    <p className="fst-italic mb-4">"{testimonial.quote}"</p>
                    <div className="d-flex align-items-center">
                      <div className="me-3">
                        <div className="rounded-circle bg-primary" style={{width: '50px', height: '50px'}}></div>
                      </div>
                      <div>
                        <h6 className="mb-0 fw-bold">{testimonial.name}</h6>
                        <small className="text-muted">{testimonial.company}</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Contact Section */}
      <section id="contact" className="py-5 bg-light">
        <div className="container">
          <div className="row">
            <div className="col-lg-6 mb-5 mb-lg-0" data-aos="fade-right">
              <h2 className="fw-bold mb-4" style={{ color: '#1a2a5a' }}>Contact Us</h2>
              <p className="lead mb-4">Have questions about commercial auto insurance? Our specialists are here to help.</p>
              
              <div className="mb-4">
                <div className="d-flex mb-3">
                  <div className="me-3" style={{ color: '#1a2a5a' }}>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                      <path d="M3 5C3 3.89543 3.89543 3 5 3H19C20.1046 3 21 3.89543 21 5V19C21 20.1046 20.1046 21 19 21H5C3.89543 21 3 20.1046 3 19V5Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      <path d="M3 9H21" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      <path d="M9 21V9" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Corporate Office</h5>
                    <p className="mb-0">123 Insurance Tower, Business District<br />Mumbai, Maharashtra 400001</p>
                  </div>
                </div>
                
                <div className="d-flex mb-3">
                  <div className="me-3" style={{ color: '#1a2a5a' }}>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                      <path d="M5 4H9L11 9L8.5 10.5C9.57096 12.6715 11.3285 14.429 13.5 15.5L15 13L20 15V19C20 19.5304 19.7893 20.0391 19.4142 20.4142C19.0391 20.7893 18.5304 21 18 21C14.0993 20.763 10.4202 19.1065 7.65683 16.3432C4.8935 13.5798 3.23705 9.90074 3 6C3 5.46957 3.21071 4.96086 3.58579 4.58579C3.96086 4.21071 4.46957 4 5 4Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Call Us</h5>
                    <p className="mb-0">Toll-free: 1800-123-4567<br />Commercial Dept: 1800-765-4321</p>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="col-lg-6" data-aos="fade-left">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body p-4">
                  <h3 className="fw-bold mb-4" style={{ color: '#1a2a5a' }}>Send Us a Message</h3>
                  <form>
                    <div className="mb-3">
                      <input type="text" className="form-control py-3" placeholder="Your Name" />
                    </div>
                    <div className="mb-3">
                      <input type="email" className="form-control py-3" placeholder="Email Address" />
                    </div>
                    <div className="mb-3">
                      <input type="text" className="form-control py-3" placeholder="Company Name" />
                    </div>
                    <div className="mb-3">
                      <textarea className="form-control py-3" rows="4" placeholder="Your Message"></textarea>
                    </div>
                    <button type="submit" className="btn w-100 py-3 fw-bold text-white" style={{ backgroundColor: '#1a2a5a' }}>Send Message</button>
                  </form>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="py-4 text-white" style={{ backgroundColor: '#1a2a5a' }}>
        <div className="container">
          <div className="row">
            <div className="col-md-6 text-center text-md-start mb-3 mb-md-0">
              <p className="mb-0">&copy; {new Date().getFullYear()} InsureWise Commercial. All rights reserved.</p>
            </div>
            <div className="col-md-6 text-center text-md-end">
              <p className="mb-0">
                <a href="#" className="text-white text-decoration-none me-3">Privacy Policy</a>
                <a href="#" className="text-white text-decoration-none">Terms of Service</a>
              </p>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default HomePage;