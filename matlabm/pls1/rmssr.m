function rms=rmssr(x,l);

%���ײв�������
%x�����׾�������������
%l���������ɷַ����غɣ�
%rmssr�����ײв



[m,n]=size(x);
r=x-l'*(l*x);
for i=1:n
    rms(i)=sqrt(r(:,i)'*r(:,i)/m);
end