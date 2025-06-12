export const initialProposalData = {
  vehicle: {
    vehicleType: "",
    vehicleNumber: "",
    chassisNumber: "",
    engineNumber: "",
    makerName: "",
    modelName: "",
    vehicleColor: "",
    fuelType: "",
    registrationDate: "",
    seatCapacity: 1,
  },
  proposal: {
    insuranceType: "",
    insuranceValidUpto: "",
    fitnessValidUpto: "",
  },
  insuranceDetails: {
    insuranceStartDate: "",
    insuranceSum: 0,
    damageInsurance: "", // Should be string not boolean
    liabilityOption: "", // Should be string not boolean
    plan: "",
  },
  documents: {
    license: null, // For License file
    rcBook: null,  // For RCBook file
    pollutionCertificate: null, // For Pollution Certificate file
  },
};
