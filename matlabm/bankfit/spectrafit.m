function [ra] = spectrafit(C,d,x0,options)

tol = 10*eps*norm(C,1)*length(C);
[m,n] = size(C);
P = zeros(1,n);
Z = 1:n;
ra = P';

ZZ=Z;
resid = d-C*ra;
w = C'*(resid);


% set up iteration criterion
outeriter = 0;
iter = 0;
itmax = 3*n;
exitflag = 1;
kkk = 0;
while any(Z) & any(w(ZZ) > tol)
   
   www = w(ZZ);
    outeriter = outeriter + 1;
   [wt,t] = max(w(ZZ));
   t = ZZ(t);
   P(1,t) = t;
   Z(t) = 0;
   PP = find(P);
   ZZ = find(Z);
   nzz = size(ZZ);
   CP(1:m,PP) = C(:,PP);
   CP(:,ZZ) = zeros(m,nzz(2));
   z = pinv(CP)*d;
   z(ZZ) = zeros(nzz(2),nzz(1));
   % inner loop to remove elements from the positive set which no longer belong
   
   while any((z(PP) <= tol))
      ttt = z(PP);
       iter = iter + 1;
      if iter > itmax
         
         exitflag = 0;
         output.iterations = outeriter;
         resnorm = sum(resid.*resid);
         ra = z;
         lambda = w;
         return
      end
      QQ = find((z <= tol) & P');
      alpha = min(ra(QQ)./(ra(QQ) - z(QQ)));
      ra = ra + alpha*(z - ra);
      ij = find(abs(ra) < tol & P' ~= 0);
      Z(ij)=ij';
      P(ij)=zeros(1,length(ij));
      PP = find(P);
      ZZ = find(Z);
      nzz = size(ZZ);
      CP(1:m,PP) = C(:,PP);
      CP(:,ZZ) = zeros(m,nzz(2));
      z = pinv(CP)*d;
      z(ZZ) = zeros(nzz(2),nzz(1));
   end
   ra = z;
   resid = d-C*ra;
   w = C'*(resid);
   kkk = kkk+1;
end






















