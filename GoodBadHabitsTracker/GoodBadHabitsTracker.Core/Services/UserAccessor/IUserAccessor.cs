﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.UserAccessor
{
    public interface IUserAccessor
    {
        Guid GetLoggedUserId();
    }
}
