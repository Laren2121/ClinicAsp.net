﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace A1Patients.Models
{
    public partial class PatientTreatment
    {
        public PatientTreatment()
        {
            PatientMedication = new HashSet<PatientMedication>();
        }

        public int PatientTreatmentId { get; set; }
        public int TreatmentId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMMM YYYY HH:mm}")]
        public DateTime DatePrescribed { get; set; }
        public string Comments { get; set; }
        public int PatientDiagnosisId { get; set; }

        public virtual PatientDiagnosis PatientDiagnosis { get; set; }
        public virtual Treatment Treatment { get; set; }
        public virtual ICollection<PatientMedication> PatientMedication { get; set; }
    }
}
